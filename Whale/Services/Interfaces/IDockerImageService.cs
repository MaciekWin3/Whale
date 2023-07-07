using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerImageService
    {
        Task<Result<List<Image>>> GetImageListAsync(CancellationToken token = default);
        Task<Result<List<ImageLayer>>> GetImageLayersAsync(string containerId, CancellationToken token = default);
    }
}