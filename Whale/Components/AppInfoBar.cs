using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Components
{
    public class AppInfoBar : StatusBar
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        private readonly IDockerUtilityService dockerUtilityService;
        public AppInfoBar()
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerContainerService = new DockerContainerService(shellCommandRunner);
            dockerUtilityService = new DockerUtilityService(shellCommandRunner);

            var dockerVersion = new StatusItem(Key.CharMask, $"Docker Version: Unknown", null);
            var containerCpuUsage = new StatusItem(Key.CharMask, "CPU: 0%", null);
            var containerMemoryUsage = new StatusItem(Key.CharMask, "Mem: 0%", null);

            Visible = true;
            Items = new StatusItem[]
            {
                new StatusItem(Key.C | Key.CtrlMask, "~CTRL-C~ Quit", () =>
                {
                    Application.RequestStop();
                }),
                dockerVersion,
                containerCpuUsage,
                containerMemoryUsage,
            };

            Application.MainLoop.Invoke(async () =>
            {
                while (true)
                {
                    var containersStats = await dockerContainerService.GetContainersStatsAsync();
                    var dockerVersionInfo = await dockerUtilityService.GetDockerVersionObjectAsync();
                    try
                    {
                        if (containersStats.IsSuccess || dockerVersionInfo.IsSuccess)
                        {
                            var containersStatsValue = containersStats.GetValue();
                            containerCpuUsage.Title = $"CPU: {containersStatsValue.CPUPerc}%";
                            containerMemoryUsage.Title = $"Mem: {containersStatsValue.MemUsage!}%";
                            dockerVersion.Title = $"Docker Version: {dockerVersionInfo.Value?.Client?.Version!}";
                        }
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
