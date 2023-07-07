using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerContainerService
    {
        Task<Result<List<Container>>> GetContainerListAsync(CancellationToken token = default);
        Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default);
        Task<Result<ContainerStats>> GetContainerStatsAsync(string containerId, CancellationToken token = default);
        Task<Result<string>> RunCommandInsideDockerContainerAsync(string containerId, string command, CancellationToken token = default);
        Task<Result> CreateContainerAsync(List<string> arguments);
    }
}