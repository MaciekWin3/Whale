using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public interface IDockerService
    {
        Task<Result<List<Container>>> GetContainerListAsync();
    }
}