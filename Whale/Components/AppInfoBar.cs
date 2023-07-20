using Terminal.Gui;

namespace Whale.Components
{
    public class AppInfoBar : StatusBar
    {
        public AppInfoBar(string version)
        {
            var scrolllock = new StatusItem(Key.CharMask, "Scroll", null);
            var appVersion = new StatusItem(Key.CharMask, "App Version: 0.0.1", null);
            var dockerVersion = new StatusItem(Key.CharMask, $"Docker Version: {version}", null);
            var containerCpuUsage = new StatusItem(Key.CharMask, "CPU: 0%", null);
            var containerMemoryUsage = new StatusItem(Key.CharMask, "Memory: 0%", null);
            var vmMemoryUsage = new StatusItem(Key.CharMask, "VM Memory: 0%", null);

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
                vmMemoryUsage,
            };
        }
    }
}
