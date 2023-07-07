using Whale.Models.Version;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerUtilityService
    {
        Task<Result<bool>> CheckIfDockerDaemonIsRunningAsync(CancellationToken token = default);
        Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default);
        Task<Result<DockerVersion>> GetDockerVersionObjectAsync(CancellationToken token = default);
    }
}