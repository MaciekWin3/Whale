using Terminal.Gui;

namespace Whale.Windows.Single.ContainerTabs
{
    public class ContainerTerminalWindow : Window
    {
        public ContainerTerminalWindow() : base()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
        }
        public void InitView()
        {
            var terminal = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                ReadOnly = true,
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
                //Text = "> "
            };

            Add(prompt);

            //Application.MainLoop.Invoke(async () =>
            //{
            //    while (true)
            //    {
            //        var logs = await dockerService.GetContainerLogsAsync(ContainerId);
            //        if (logs.Value.std == cache)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            textField.Text = logs?.Value.std ?? string.Empty;
            //            if (logs?.Value.std is not null)
            //            {
            //                cache = logs?.Value.std;
            //            }
            //        }
            //    }
            //});

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    terminal.Text += prompt.Text + "\n";
                    prompt.Text = "> ";
                }
            };
        }
    }
}
