using System.Text.Json;
using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public class DockerService : IDockerService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }
        public DockerService(ShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<List<ContainerDTO>>> GetContainerListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "ps", "-a" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = Mapper.MapCommandToContainerList(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<ContainerDTO>>("No containers found");
            }
            return Result.Fail<List<ContainerDTO>>("Command failed");
        }

        public async Task<Result<List<VolumeDTO>>> GetVolumeListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "volume", "ls" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var volumes = Mapper.MapCommandToVolumeList(result.Value.std);
                    return volumes;
                }
                return Result.Fail<List<VolumeDTO>>("No containers found");
            }
            return Result.Fail<List<VolumeDTO>>("Command failed");
        }

        public async Task<Result<List<ImageDTO>>> GetImageListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "images" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var images = Mapper.MapCommandToImageList(result.Value.std);
                    return images;
                }
                return Result.Fail<List<ImageDTO>>("No images found");
            }
            return Result.Fail<List<ImageDTO>>("Command failed");
        }

        public async Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default)
        {
            var result = await shellCommandRunner
                .RunCommandAsync("docker", new[] { "inspect", id }, token);

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
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "info" }, token);
            if (result.IsSuccess)
            {
                return Result.Ok(true);
            }
            return Result.Fail<bool>("Docker daemon is not running");
        }
    }
}
