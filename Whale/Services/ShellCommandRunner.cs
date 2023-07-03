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
        public async Task<Result<(string std, string error)>> ObserveCommandAsync(string command, string[] arguments, CancellationToken token = default)
        {
            var stdOutSb = new StringBuilder();
            var stdErrSb = new StringBuilder();
            var joinedArguments = string.Join(" ", arguments).Trim();

            try
            {
                var cmd = Cli.Wrap(command)
                            .WithArguments(joinedArguments)
                            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutSb))
                            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrSb));

                await cmd.Observe().ForEachAsync(cmdEvent =>
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Process started; ID: {started.ProcessId}");
                            Console.ResetColor();
                            break;
                        case StandardOutputCommandEvent stdOut:
                            stdOutSb.AppendLine($"Out> {stdOut.Text}");
                            break;
                        case StandardErrorCommandEvent stdErr:
                            stdErrSb.AppendLine($"Out> {stdErr.Text}");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Err> {stdErr.Text}");
                            Console.ResetColor();
                            break;
                        case ExitedCommandEvent exited:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"Process exited; Code: {exited.ExitCode}");
                            Console.ResetColor();
                            break;
                    }
                });
            }
            catch (Exception e)
            {
                return Result.Fail<(string, string)>(e.Message);
            }

            return stdErrSb.Length > 0
                ? Result.Fail<(string, string)>(stdErrSb.ToString())
                : Result.Ok((stdOutSb.ToString(), stdErrSb.ToString()));
        }
    }
}
