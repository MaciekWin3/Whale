using Terminal.Gui;

namespace Whale
{
    public class MainWindow : Window
    {
        public MainWindow() : base("Whale Dashboard")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
            InitWindow();
        }

        public void InitWindow()
        {
            var text = new Label("")
            {
                X = 0,
                Y = 3
            };
            Add(text);

            var text2 = new Label("")
            {
                X = 0,
                Y = 10
            };
            Add(text2);

            Task.Run(() =>
            {
                text.Text = ShellCommandRunner.RunCommand("docker ps -a");
                Application.Refresh();
            });
            Task.Run(() =>
            {
                text2.Text = ShellCommandRunner.RunCommand("docker images");
                Application.Refresh();
            });
        }
    }
}
