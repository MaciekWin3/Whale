using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Containers.Tabs
{
    public sealed class ContainerLogsWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        public string ContainerId { get; set; }
        public ContainerLogsWindow(string containerId) : base()
        {
            ContainerId = containerId;
            shellCommandRunner = new ShellCommandRunner();
            dockerContainerService = new DockerContainerService(shellCommandRunner);
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            ColorScheme = Colors.Base;
        }

        public void InitView()
        {
            var textField = new TextView
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
                DesiredCursorVisibility = CursorVisibility.Vertical,
                ReadOnly = true,
            };
            int idx = textField.Lines;
            textField.ScrollTo(idx - textField.Bounds.Height - 1);
            Add(textField);

            Application.MainLoop.Invoke(async () =>
            {
                string? cache = string.Empty;
                while (true)
                {
                    var logs = await dockerContainerService.GetContainerLogsAsync(ContainerId);
                    if (logs.Value.std == cache)
                    {
                        continue;
                    }
                    else
                    {
                        textField.Text = logs?.Value.std ?? string.Empty;
                        if (logs?.Value.std is not null)
                        {
                            cache = logs?.Value.std;
                        }
                        int idx = textField.Lines;
                        textField.ScrollTo(idx - textField.Bounds.Height - 1);
                    }
                }
            });
        }
    }
}
