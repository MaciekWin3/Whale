using CliWrap;
using CliWrap.EventStream;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Whale.Utils;

namespace Whale.Services
{
    public class ShellCommandRunner : IShellCommandRunner
    {
        public async Task<Result<(string std, string error)>> RunCommandAsync(string command, string[] arguments, CancellationToken token = default)
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
                //.ExecuteBufferedAsync(token);
                    .ExecuteAsync(token);
            }
            catch (Exception e)
            {
                return Result.Fail<(string, string)>(e.Message);
            }

            return stdErr.Length > 0
                ? Result.Fail<(string, string)>(stdErr.ToString())
                : Result.Ok((stdOut.ToString(), stdErr.ToString()));
        }

        // Handle windows, linux and mac
        public async Task<Result<(string std, string error)>> RunCommandAsync(string arguments, CancellationToken token = default)
        {
            string command;
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                command = "cmd.exe";
                //command = "powershell.exe";
                arguments = $"/c {arguments}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                command = "/bin/bash";
                arguments = $"-c {arguments}";
            }
            else
            {
                return Result.Fail<(string, string)>("Unsupported OS");
            }

            try
            {
                await Cli.Wrap(command)
                    .WithArguments(arguments)
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErr))
                    .ExecuteAsync(token);
            }
            catch (Exception e)
            {
                return Result.Fail<(string, string)>(e.Message);
            }

            return stdErr.Length > 0
                ? Result.Fail<(string, string)>(stdErr.ToString())
                : Result.Ok((stdOut.ToString(), stdErr.ToString()));
        }

        // i want to try out observe from cliwrap
        public async Task<Result<(int processId, string std, string error, int exitCode)>> ObserveCommandAsync
            (string arguments, Action<string> lambda = null!, CancellationToken token = default)
        {
            string command;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                command = "powershell.exe";
                arguments = $"/c {arguments}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                command = "/bin/bash";
                arguments = $"-c {arguments}";
            }
            else
            {
                return Result.Fail<(int, string, string, int)>("Unsupported OS");
            }

            int processId = 0;
            int exitCode = 0;
            var stdOutSb = new StringBuilder();
            var stdErrSb = new StringBuilder();
            var joinedArguments = string.Join(" ", arguments).Trim();

            try
            {
                var cmd = Cli.Wrap(command)
                            .WithArguments(joinedArguments)
                            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutSb))
                            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrSb));

                await cmd.Observe(cancellationToken: token).ForEachAsync(cmdEvent =>
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            processId = started.ProcessId;
                            break;
                        case StandardOutputCommandEvent stdOut:
                            if (lambda is not null)
                            {
                                lambda(stdOut.Text);
                            }
                            stdOutSb.AppendLine(stdOut.Text);
                            break;
                        case StandardErrorCommandEvent stdErr:
                            stdErrSb.AppendLine(stdErr.Text);
                            if (lambda is not null)
                            {
                                lambda(stdErr.Text);
                            }
                            break;
                        case ExitedCommandEvent exited:
                            exitCode = exited.ExitCode;
                            break;
                    }
                });
            }
            catch (Exception e)
            {
                return Result.Fail<(int, string, string, int)>(e.Message);
            }

            return Result.Ok((processId, stdOutSb.ToString(), stdErrSb.ToString(), exitCode));
        }
    }
}
