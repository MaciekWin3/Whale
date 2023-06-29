using Terminal.Gui;
using Whale.Services;

namespace Whale.Components
{
    public class AppInfoBar
    {
        public static async Task<StatusBar> Create()
        {
            var shellCommandRunner = new ShellCommandRunner();
            var dockerService = new DockerService(shellCommandRunner);
            var dockerInfo = await dockerService.GetDockerVersionObjectAsync();

            var capslock = new StatusItem(Key.CharMask, "Caps", null);
            var numlock = new StatusItem(Key.CharMask, "Num", null);
            var scrolllock = new StatusItem(Key.CharMask, "Scroll", null);
            var appVersion = new StatusItem(Key.CharMask, "App Version: 0.1.0", null);
            var dockerVersion = new StatusItem(Key.CharMask, $"Docker Version {dockerInfo.Value.Client.Version}", null);

            var statusBar = new StatusBar()
            {
                Visible = true,
            };
            statusBar.Items = new StatusItem[]
            {
                new StatusItem(Key.C | Key.CtrlMask, "~CTRL-C~ Quit", () =>
                {
                    Application.RequestStop();
                }),
                appVersion,
                dockerVersion
            };

            return statusBar;
        }
    }
}
