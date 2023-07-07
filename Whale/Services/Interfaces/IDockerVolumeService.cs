using Whale.Models;
using Whale.Utils;

namespace Whale.Services.Interfaces
{
    internal interface IDockerVolumeService
    {
        Task<Result<List<Volume>>> GetVolumeListAsync(CancellationToken token = default);
    }
}