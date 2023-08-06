using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Containers.Tabs
{
    public sealed class ContainerTerminalWindow : Window
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        public string ContainerId { get; set; }
        TextView terminal = null!;
        TextField prompt = null!;
        public ContainerTerminalWindow(string containerId)
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

            var line = new LineView()
            {
                X = 0,
                Y = Pos.Bottom(terminal),
                Width = Dim.Fill(),
                Height = 1,
            };

            prompt = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(line),
                Width = Dim.Fill(),
                Height = 1,
            };

            Add(terminal, line, prompt);
            FocusNext();
        }

        public void HandleKeyPress(KeyEvent keyEvent)
        {
            switch (keyEvent.Key)
            {
                case Key.Enter:
                    HandleInput();
                    break;
                default:
                    break;
            }
        }

        public void HandleInput()
        {
            var command = prompt.Text.ToString();
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            terminal.Text += prompt.Text + "\n";
            Application.MainLoop.Invoke(async () =>
            {
                var result = await dockerContainerService.RunCommandInsideDockerContainerAsync(ContainerId, command);
                if (result.IsSuccess)
                {
                    terminal.Text += result.Value + '\n';
                }
                else
                {
                    terminal.Text += result.Error + "\n";
                }
                int idx = terminal.Lines;
                terminal.ScrollTo(idx - terminal.Bounds.Height - 1);
            });
            prompt.Text = "";
        }
    }
}
