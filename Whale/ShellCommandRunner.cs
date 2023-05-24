﻿using CliWrap;
using System.Text;
using Whale.Utils;

namespace Whale
{
    public static class ShellCommandRunner
    {
        public static async Task<Result<(string std, string error)>> RunCommandAsync(string command, params string[] arguments)
        {
            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            var joinedArguments = string.Join(" ", arguments);

            await Cli.Wrap(command).WithArguments(joinedArguments)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOut))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErr))
                .ExecuteAsync();

            return stdErr.Length > 0
                ? Result.Fail<(string, string)>(stdErr.ToString())
                : Result.Ok((stdOut.ToString(), stdErr.ToString()));
        }
    }
}