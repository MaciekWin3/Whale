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
