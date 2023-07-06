using Terminal.Gui;
using Whale.Services;

namespace Whale.Components
{
    public class TerminalDialog : Dialog
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public TerminalDialog()
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
            X = Pos.Center();
            Y = Pos.Center();
            Width = Dim.Percent(40);
            Height = Dim.Percent(40);
            Border = new Border
            {
                BorderStyle = BorderStyle.Rounded,
                Effect3D = false,
                Title = "Terminal",
                Padding = new Thickness(1, 0, 1, 0),
            };
            InitView();
        }

        private void InitView()
        {
            var label = new Label("Run command:")
            {
                X = 0,
                Y = 1,
            };
            var terminal = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(label),
                Width = Dim.Fill()
            };

            Action<string> lambda = (s) =>
            {
                Application.MainLoop.Invoke(() =>
                {
                    terminal.Text = s;
                });
            };


            var runButton = new Button("Run");
            runButton.Clicked += async () =>
            {
                var command = terminal.Text.ToString();
                //var result = await shellCommandRunner.RunCommandAsync(command);
                var result = await shellCommandRunner.ObserveCommandAsync("ping", new[] { "wp.pl" }, lambda);
                if (result.IsFailure)
                {
                    MessageBox.ErrorQuery(50, 7, "Error", result.Error, "Ok");
                }
                else
                {
                    MessageBox.Query(50, 7, "Info", result.Value.std, "Ok");
                }
            };

            var exitButton = new Button("Exit");
            exitButton.Clicked += () =>
            {
                Application.RequestStop();
            };

            Add(label, terminal);
            AddButton(runButton);
            AddButton(exitButton);
        }

        public void ShowDialog()
        {
            Application.Run(this);
        }
    }
}
