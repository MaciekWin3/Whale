using Whale.Models;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Services
{
    public class DockerContainerService : IDockerContainerService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerContainerService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<List<Container>>> GetContainerListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker",
                new[] { "container", "ls", "--all", "--format", "json" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = Mapper.MapCommandToDockerObjectList<Container>(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<Container>>("No containers found");
            }
            return Result.Fail<List<Container>>("Command failed");
        }

        public async Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default)
        {
            // docker logs -f ???
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "logs", containerId }, default);
            if (result.IsSuccess)
            {
                return result.Value;
            }
            return Result.Fail<(string std, string err)>("Command failed");
        }

        public async Task<Result<ContainerStats>> GetContainerStatsAsync(string containerId, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "stats", "--no-stream", "--format", "json", containerId }, default);
            if (result.IsSuccess)
            {
                var stats = Mapper.MapCommandToDockerObject<ContainerStats>(result.Value.std);
                if (stats.IsSuccess && stats.Value is not null)
                {
                    return Result.Ok(stats.Value);
                }
            }
            return Result.Fail<ContainerStats>("Command failed");
        }

        public async Task<Result<string>> RunCommandInsideDockerContainerAsync(string containerId, string command, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "exec", containerId, command }, token);
            if (result.IsSuccess)
            {
                return Result.Ok(result.Value.std);
            }
            return Result.Fail<string>("Command failed");
        }

        public async Task<Result> CreateContainerAsync(List<string> arguments)
        {
            var commandParameters = arguments.Prepend("run").ToArray();
            var result = await shellCommandRunner.RunCommandAsync("docker", commandParameters, default);
            if (result.IsSuccess)
            {
                return result;
            }
            return Result.Fail<(string std, string err)>("Command failed");
        }
    }
}
