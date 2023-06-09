using CliWrap;
using System.Text;
using Whale.Utils;

namespace Whale.Services
{
    public class ShellCommandRunner : IShellCommandRunner
    {
        public async Task<Result<(string std, string error)>> RunCommandAsync(string command, params string[] arguments)
        {
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            var joinedArguments = string.Join(" ", arguments).Trim();

            try
            {
                await Cli.Wrap(command)
                    .WithArguments(joinedArguments)
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErr))
                    .ExecuteAsync();
            }
            catch (Exception e)
            {
                return Result.Fail<(string, string)>(e.Message);
            }

            return stdErr.Length > 0
                ? Result.Fail<(string, string)>(stdErr.ToString())
                : Result.Ok((stdOut.ToString(), stdErr.ToString()));
        }
    }
}
