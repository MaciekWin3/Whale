using System.Runtime.InteropServices;

namespace Whale.Utils
{
    public static class ShellCommandRunner
    {
        public static string RunCommand(string command)
        {
            return "Dupa";
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
