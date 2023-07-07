using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IDockerImageService
    {
        Task<Result<List<Image>>> GetImageListAsync(CancellationToken token = default);
    }
}