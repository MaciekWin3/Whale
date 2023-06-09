using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public interface IDockerService
    {
        Task<Result<List<ContainerDTO>>> GetContainerListAsync();
        Task<Result<List<VolumeDTO>>> GetVolumeListAsync();
        Task<Result<List<ImageDTO>>> GetImageListAsync();
        Task<Result<T>> GetDockerObjectInfoAsync<T>(string id);
    }
}