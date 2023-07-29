using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Components
{
    public class AppInfoBar : StatusBar
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        public AppInfoBar(string version)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerContainerService = new DockerContainerService(shellCommandRunner);

            var appVersion = new StatusItem(Key.CharMask, "App Version: 0.0.1", null);
            var dockerVersion = new StatusItem(Key.CharMask, $"Docker Version: {version}", null);
            var containerCpuUsage = new StatusItem(Key.CharMask, "CPU: 0%", null);
            var containerMemoryUsage = new StatusItem(Key.CharMask, "Mem: 0%", null);

            Visible = true;
            Items = new StatusItem[]
            {
                new StatusItem(Key.C | Key.CtrlMask, "~CTRL-C~ Quit", () =>
                {
                    Application.RequestStop();
                }),
                appVersion,
                dockerVersion,
                containerCpuUsage,
                containerMemoryUsage,
            };

            Application.MainLoop.Invoke(async () =>
            {
                while (true)
                {
                    var result = await dockerContainerService.GetContainersStatsAsync();
                    try
                    {
                        containerCpuUsage.Title = $"CPU: {result.Value.CPUPerc}%";
                        containerMemoryUsage.Title = $"Mem: {result.Value.MemUsage}%";
                    }
                    catch
                    {
                        containerCpuUsage.Title = $"CPU: 0%";
                        containerMemoryUsage.Title = $"Mem: 0%";
                    }
                }
            });
        }
    }
}
