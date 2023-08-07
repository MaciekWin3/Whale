using System.Text.Json;
using Whale.Models.Version;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Services
{
    public class DockerUtilityService : IDockerUtilityService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerUtilityService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }
        public DockerUtilityService(ShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<DockerVersion>> GetDockerVersionObjectAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker version --format json", default);
            if (result.IsSuccess)
            {
                var stats = Mapper.MapCommandToDockerObject<DockerVersion>(result.Value.std);
                if (stats.IsSuccess && stats.Value is not null)
                {
                    return Result.Ok(stats.Value);
                }
            }
            return Result.Fail<DockerVersion>("Command failed");
        }

        public async Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default)
        {
            var result = await shellCommandRunner
                .RunCommandAsync($"docker inspect {id} --format json", token);

            if (result.IsFailure)
            {
                return Result.Fail<T>("Command failed");
            }

            JsonElement x = JsonDocument.Parse(result.Value.std).RootElement;

            JsonElement firstElement = x
                .EnumerateArray()
                .FirstOrDefault();

            var rawText = firstElement.GetRawText();
            var dockerObject = JsonSerializer.Deserialize<T>(rawText, new JsonSerializerOptions() { });

            if (dockerObject is null)
            {
                return Result.Fail<T>("Failed to deserialize object");
            }

            return Result.Ok(dockerObject);
        }

        public async Task<Result<bool>> CheckIfDockerDaemonIsRunningAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker ps", token);
            if (result.IsSuccess)
            {
                return Result.Ok(true);
            }
            return Result.Fail<bool>("Docker daemon is not running");
        }
    }
}
