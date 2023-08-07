using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerVolumeService
    {
        Task<Result<List<Volume>>> GetVolumeListAsync(CancellationToken token = default);
        Task<Result<List<Container>>> GetVolumesContainersListAsync(string containerId, CancellationToken token = default);
        Task<Result> DeleteVolumeAsync(string volumeId, CancellationToken token = default);
    }
}