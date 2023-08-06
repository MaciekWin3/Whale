using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Components
{
    public class TerminalDialog : Dialog
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public TerminalDialog()
        {
            shellCommandRunner = new ShellCommandRunner();
            X = Pos.Center();
            Y = Pos.Center();
            Width = Dim.Percent(80);
            Height = Dim.Percent(80);
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
            var terminal = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                WordWrap = true,
                ReadOnly = true,
                ColorScheme = new ColorScheme()
                {
                    Disabled = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    HotFocus = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    Focus = Application.Driver.MakeAttribute(Color.Green, Color.Black),
                    Normal = Application.Driver.MakeAttribute(Color.Green, Color.Black)
                },
            };

            var line = new LineView()
            {
                X = 0,
                Y = Pos.Bottom(terminal),
                Width = Dim.Fill(),
                Height = 1,
            };

            var prompt = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(line),
                Width = Dim.Fill() - 1,
                Height = 1,
            };

            Action<string> lambda = (s) =>
            {
                terminal.Text += s + '\n';
            };

            KeyPress += async (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    var args = prompt.Text.ToString();
                    e.Handled = true;
                    prompt.Text = "";
                    if (string.IsNullOrEmpty(args))
                    {
                        return;
                    }
                    //var result = await shellCommandRunner.ObserveCommandAsync(args, lambda);
                    var result = await shellCommandRunner.RunCommandAsync(args);
                    if (result.IsSuccess)
                    {
                        terminal.Text += result.Value.std + '\n';
                    }
                    else
                    {
                        terminal.Text += result.Error + '\n';
                    }
                    int idx = terminal.Lines;
                    terminal.ScrollTo(idx - terminal.Bounds.Height - 1);
                }
            };

            Add(terminal, line, prompt);
            FocusNext();
        }

        public void ShowDialog()
        {
            Application.Run(this);
        }
    }
}
