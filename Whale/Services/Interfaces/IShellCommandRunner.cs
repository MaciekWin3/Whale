using Whale.Utils;

namespace Whale.Services.Interfaces
{
    public interface IShellCommandRunner
    {
        Task<Result<(string std, string error)>> RunCommandAsync(string command, string[] arguments, CancellationToken cancellationToken = default);
        Task<Result<(string std, string error)>> RunCommandAsync(string arguments, CancellationToken token = default);
        Task<Result<(int processId, string std, string error, int exitCode)>> ObserveCommandAsync
            (string arguments, Action<string> lambda = null!, CancellationToken token = default);
    }
}