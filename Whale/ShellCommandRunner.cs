using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Whale
{
    public static class ShellCommandRunner
    {
        public static void RunCommand(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = GetShellFileName(),
                Arguments = GetShellArguments(command),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(output);
        }

        private static string GetShellFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "cmd.exe";
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
