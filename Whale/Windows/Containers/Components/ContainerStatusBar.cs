using Terminal.Gui;

namespace Whale.Windows.Containers.Components
{
    public class ContainerStatusBar : StatusBar
    {
        public ContainerStatusBar()
        {
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
    }
}
