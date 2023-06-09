using Whale.Utils;

namespace Whale.Services
{
    public interface IShellCommandRunner
    {
        Task<Result<(string std, string error)>> RunCommandAsync(string command, params string[] arguments);
    }
}