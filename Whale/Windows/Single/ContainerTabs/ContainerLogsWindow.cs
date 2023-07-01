using Terminal.Gui;
using Whale.Services;

namespace Whale.Windows.Single.ContainerTabs
{
    public sealed class ContainerLogsWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public string ContainerId { get; set; }
        public ContainerLogsWindow(string containerId) : base()
        {
            ContainerId = containerId;
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
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
                ReadOnly = true
            };
            Add(textField);

            Application.MainLoop.Invoke(async () =>
            {
                string? cache = string.Empty;
                while (true)
                {
                    var logs = await dockerService.GetContainerLogsAsync(ContainerId);
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
