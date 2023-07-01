using Terminal.Gui;
using Whale.Services;

namespace Whale.Windows.Single.ContainerTabs
{
    public sealed class ContainerTerminalWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public string ContainerId { get; set; }
        TextView terminal = null!;
        public ContainerTerminalWindow(string containerId) : base()
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
            terminal = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                ReadOnly = true,
                ColorScheme = new ColorScheme()
                {
                    Disabled = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    HotFocus = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    Focus = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    Normal = Application.Driver.MakeAttribute(Color.Green, Color.Black)
                }
            };
            Add(terminal);

            var line = new LineView()
            {
                X = 0,
                Y = Pos.Bottom(terminal),
                Width = Dim.Fill(),
                Height = 1,
            };
            Add(line);

            var prompt = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(line),
                Width = Dim.Fill(),
                Height = 1,
            };

            Add(prompt);

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    terminal.Text += prompt.Text + "\n";
                    HandleInput(prompt.Text.ToString());
                    prompt.Text = "";
                }
            };
        }

        private void HandleInput(string command)
        {
            Application.MainLoop.Invoke(async () =>
            {
                var result = await dockerService.RunCommandInsideDockerContainerAsync(ContainerId, command);
                if (result.IsSuccess)
                {
                    terminal.Text += result.Value;
                }
                int idx = terminal.Lines;
                terminal.ScrollTo(idx - terminal.Bounds.Height - 1);
            });
        }
    }
}
