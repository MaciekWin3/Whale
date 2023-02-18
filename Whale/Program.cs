using System.Diagnostics;

RunProcess("docker ps -a");

static void RunProcess(string command)
{
    Process process = new();
    process.StartInfo.FileName = "pwsh.exe";
    process.StartInfo.Arguments = $"/c {command}";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();

    string output = process.StandardOutput.ReadToEnd();

    process.WaitForExit();
    Console.WriteLine(output);
}