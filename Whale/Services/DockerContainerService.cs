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

        public async Task<Result<List<Container>>> GetContainerListAsync(bool showAll = true, CancellationToken token = default)
        {
            var result = await shellCommandRunner
                .RunCommandAsync(showAll ?
                "docker container ls --all --format json" : "docker container ls --format json",
                token);

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

        public async Task<Result<DockerStats>> GetContainersStatsAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker",
                new[] { "container", "stats", "--format", "json", "--no-stream" }, token);

            if (result.IsSuccess)
            {
                var containersStats = Mapper.MapCommandToDockerObjectList<ContainerStats>(result.Value.std);
                var dockerStats = new DockerStats();

                var containerStatsList = containersStats.GetValue();

                foreach (var containerStats in containerStatsList)
                {
                    if (int.TryParse(containerStats.PIDs, out int pids))
                    {
                        dockerStats.PIDs += pids;
                    }

                    if (containerStats.CPUPerc is not null && float.TryParse(containerStats.CPUPerc.Replace("%", ""), out float cpuPerc))
                    {
                        dockerStats.CPUPerc += cpuPerc;
                    }

                    if (containerStats.MemPerc is not null && float.TryParse(containerStats.MemPerc.Replace("%", ""), out float memPerc))
                    {
                        dockerStats.MemPerc += memPerc;
                    }

                    // Parse and add MemUsage and MemLimit
                    if (containerStats.MemUsage is not null)
                    {
                        var (memUsageValue, _) = ParseMemoryValue(containerStats.MemUsage);
                        dockerStats.MemUsage += memUsageValue;
                        dockerStats.MemLimit += ParseMemoryValue(containerStats.MemUsage).Value;
                    }


                    // Parse and add NetIO
                    if (containerStats.NetIO is not null)
                    {
                        var (netInputValue, _, netOutputValue, _) = ParseNetworkValue(containerStats.NetIO);
                        dockerStats.NetInput += netInputValue;
                        dockerStats.NetOutput += netOutputValue;
                    }

                    // Parse and add BlockIO
                    if (containerStats.BlockIO is not null)
                    {
                        var (blockInputValue, _, blockOutputValue, _) = ParseBlockValue(containerStats.BlockIO);
                        dockerStats.BlockInput += blockInputValue;
                        dockerStats.BlockOutput += blockOutputValue;
                    }
                }
                return Result.Ok(dockerStats);
            }
            return Result.Fail<DockerStats>("Command failed");
        }

        public async Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default)
        {
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

        public async Task<Result> CreateContainerAsync(string parameters, bool shouldRun = true, CancellationToken token = default)
        {
            string command = $"docker {(shouldRun ? "run -d" : "create")} {parameters}";
            var result = await shellCommandRunner.RunCommandAsync(command, default);
            if (result.IsSuccess)
            {
                return result;
            }
            return Result.Fail<(string std, string err)>("Command failed");
        }


        // Helper methods to parse different units (KB, MB, GB, etc.)

        // Parses memory value (e.g., 428KiB) to value and unit.
        private (float Value, string Unit) ParseMemoryValue(string memoryValue)
        {
            var parts = memoryValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && float.TryParse(parts[0], out float value))
            {
                return (value, parts[1].Trim());
            }
            else
            {
                return (0, "");
            }
        }

        // Parses network value (e.g., 1.39kB / 0B) to input and output values and units.
        private (float Input, string InputUnit, float Output, string OutputUnit) ParseNetworkValue(string networkValue)
        {
            var parts = networkValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var (inputValue, inputUnit) = ParseMemoryValue(parts[0]);
                var (outputValue, outputUnit) = ParseMemoryValue(parts[1]);
                return (inputValue, inputUnit, outputValue, outputUnit);
            }
            else
            {
                return (0, "", 0, "");
            }
        }

        // Parses block value (e.g., 0B / 0B) to input and output values and units.
        private (int Input, string InputUnit, int Output, string OutputUnit) ParseBlockValue(string blockValue)
        {
            var parts = blockValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var (inputValue, inputUnit) = ParseMemoryValue(parts[0]);
                var (outputValue, outputUnit) = ParseMemoryValue(parts[1]);
                return ((int)inputValue, inputUnit, (int)outputValue, outputUnit);
            }
            else
            {
                return (0, "", 0, "");
            }
        }
    }
}
