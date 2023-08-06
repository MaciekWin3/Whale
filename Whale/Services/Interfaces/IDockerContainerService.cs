using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerContainerService
    {
        Task<Result<List<Container>>> GetContainerListAsync(bool showAll = true, CancellationToken token = default);
        Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default);
        Task<Result<ContainerStats>> GetContainerStatsAsync(string containerId, CancellationToken token = default);
        Task<Result<string>> RunCommandInsideDockerContainerAsync(string containerId, string command, CancellationToken token = default);
        Task<Result> CreateContainerAsync(string parameters, bool shouldRun = true, CancellationToken token = default);
        Task<Result<DockerStats>> GetContainersStatsAsync(CancellationToken token = default);
    }
}