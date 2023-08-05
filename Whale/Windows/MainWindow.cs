using Terminal.Gui;
using Whale.Components;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Windows.Containers;
using Whale.Windows.Images;
using Whale.Windows.Volumes;

namespace Whale.Windows
{
    public sealed class MainWindow : Window
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerUtilityService dockerUtilityService;
        private readonly ContainerListWindow containerWindow;
        private readonly ImageListWindow imageWindow;
        private readonly VolumeListWindow volumeWindow;
        private TabView tabView = null!;

        public MainWindow() : base("Whale Dashboard")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.Rounded,
                Effect3D = false,
                Title = "Whale Dashboard",
                Padding = new Thickness(1, 0, 1, 0),
            };
            shellCommandRunner = new ShellCommandRunner();
            dockerUtilityService = new DockerUtilityService(shellCommandRunner);
            containerWindow = new ContainerListWindow(this);
            imageWindow = new ImageListWindow(this);
            volumeWindow = new VolumeListWindow(this);
        }

        //public static async Task<Window> CreateAsync()
        public static Window CreateAsync()
        {
            var window = new MainWindow();
            //await window.InitWindow();
            window.InitWindow();
            Application.Refresh();
            return window;
        }

        public void InitWindow()
        {
            tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Fill(),
            };

            // Tabs
            tabView.AddTab(new TabView.Tab("Containers", containerWindow), true);
            tabView.AddTab(new TabView.Tab("Images", imageWindow), false);
            tabView.AddTab(new TabView.Tab("Volumes", volumeWindow), false);


            // Shortcuts
            KeyPress += (e) =>
            {
                // do apttern matchin on key press
                switch (e.KeyEvent.Key)
                {
                    case (Key.Tab):
                        var tabs = tabView.Tabs.Count;
                        if (tabView.SelectedTab == tabView.Tabs.ToArray()[tabs - 1])
                        {
                            tabView.SelectedTab = tabView.Tabs.ToArray()[0];
                        }
                        else
                        {
                            tabView.SwitchTabBy(1);
                        }
                        e.Handled = true;
                        break;
                    case (Key.t):
                        OpenTerminalDialog();
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            };

            tabView.Style.ShowBorder = true;
            tabView.ApplyStyleChanges();
            Add(tabView);

            Application.MainLoop.Invoke(async () =>
            {
                var isDockerDaemonRunning = await dockerUtilityService.CheckIfDockerDaemonIsRunningAsync();
                if (isDockerDaemonRunning.IsFailure)
                {
                    MessageBox.ErrorQuery(50, 7, "Error", "Docker daemon is not running", "Ok");
                }
            });
        }

        public string GetSelectedTab()
        {
            return (string)tabView.SelectedTab.Text;
        }

        public void OpenTerminalDialog()
        {
            var terminal = new TerminalDialog();
            terminal.ShowDialog();
        }
    }
}
