using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Containers.Components
{
    public sealed class ContainerStatusBar : StatusBar
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerImageService dockerImageService;
        public ContainerStatusBar()
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerImageService = new DockerImageService(shellCommandRunner);

            var containerCpuUsage = new StatusItem(Key.CharMask, "CPU: 0%", null);
            var containerMemoryUsage = new StatusItem(Key.CharMask, "Memory: 0%", null);
            var vmMemoryUsage = new StatusItem(Key.CharMask, "VM Memory: 0%", null);

            Visible = true;
            Items = new StatusItem[]
            {
                containerCpuUsage,
                containerMemoryUsage,
                vmMemoryUsage
            };
        }

        public void InitView()
        {

        }
    }
}
