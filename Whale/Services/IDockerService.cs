using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public interface IDockerService
    {
        Task<Result<List<Container>>> GetContainerListAsync(CancellationToken token = default);
        Task<Result<List<Volume>>> GetVolumeListAsync(CancellationToken token = default);
        Task<Result<List<Image>>> GetImageListAsync(CancellationToken token = default);
        Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default);
        Task<Result> CreateContainerAsync(List<string> arguments);
        Task<Result<string>> RunCommandInsideDockerContainerAsync(string containerId, string command, CancellationToken token = default);
        Task<Result<ContainerStats>> GetContainerStatsAsync(string containerId, CancellationToken token = default);
        Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default);
        Task<Result<bool>> CheckIfDockerDaemonIsRunningAsync(CancellationToken token = default);
    }
}