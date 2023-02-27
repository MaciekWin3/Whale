using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Whale
{
    public static class ShellCommandRunner
    {
        public static string RunCommand(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = GetShellFileName(),
                Arguments = GetShellArguments(command),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                return "Error!";
            }

            process.WaitForExit();
            return output;
            //Console.WriteLine(output);
        }

        public static async Task<string> RunShellCommandAsync(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = GetShellFileName(),
                Arguments = GetShellArguments(command),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = new Process { StartInfo = processInfo };

            // Set up event handlers for stdout and stderr data
            var tcsOutput = new TaskCompletionSource<string>();
            var tcsError = new TaskCompletionSource<string>();
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null)
                {
                    tcsOutput.TrySetResult(null);
                }
                else
                {
                    tcsOutput.TrySetResult(e.Data);
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data == null)
                {
                    tcsError.TrySetResult(null);
                }
                else
                {
                    tcsError.TrySetResult(e.Data);
                }
            };

            // Start the process and begin reading output
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for output or error data, whichever comes first
            var output = await Task.WhenAny(tcsOutput.Task, tcsError.Task);

            // Check for errors and return output or error message
            if (output == tcsError.Task)
            {
                return "Error!";
            }
            else
            {
                return await output;
            }
        }


        private static string GetShellFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "pwsh.exe";
            }
            else
            {
                return "/bin/bash";
            }
        }

        private static string GetShellArguments(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"/c \"{command}\"";
            }
            else
            {
                return $"-c \"{command}\"";
            }
        }
    }
}
