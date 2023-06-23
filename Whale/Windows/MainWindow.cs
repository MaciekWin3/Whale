using System.Globalization;
using Terminal.Gui;
using Whale.Services;
using Whale.Windows.Lists;

namespace Whale.Windows
{
    public class MainWindow : Window, IDisposable
    {

        private ContextMenu contextMenu = new();
        private MenuItem miUseSubMenusSingleFrame = null!;
        private bool useSubMenusSingleFrame;
        private readonly ShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        private readonly Dictionary<string, Delegate> events;
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
                Title = "Whale Dashboard"
            };
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);

            events = new Dictionary<string, Delegate>
            {
                { "Update", new Action(() => Application.RequestStop()) },
                { "Quit", new Action(() => Application.RequestStop()) },
                { nameof(ShowContextMenu), new Action<int, int>(ShowContextMenu) },
            };
            containerWindow = new ContainerListWindow(events, this);
            imageWindow = new ImageListWindow(ShowContextMenu, this);
            volumeWindow = new VolumeListWindow(ShowContextMenu, this);
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
            ConfigureContextMenu();

            tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Fill(),
            };

            // Tabs
            tabView.AddTab(new TabView.Tab("Containers", containerWindow), false);
            tabView.AddTab(new TabView.Tab("Images", imageWindow), false);
            tabView.AddTab(new TabView.Tab("Volumes", volumeWindow), false);

            tabView.SelectedTabChanged += (a, e) =>
            {
                if (e.NewTab.Text == "Containers")
                {
                }
                else if (e.NewTab.Text == "Images")
                {
                }
                else if (e.NewTab.Text == "Volumes")
                {
                }
            };
            tabView.Style.ShowBorder = true;
            tabView.ApplyStyleChanges();

            Add(tabView);
            Application.MainLoop.Invoke(async () =>
            {
                var isDockerDaemonRunning = await dockerService.CheckIfDockerDaemonIsRunningAsync();
                if (isDockerDaemonRunning.IsFailure)
                {
                    MessageBox.ErrorQuery(50, 7, "Error", "Docker daemon is not running", "Ok");
                }
            });
        }

        public string GetSelectedTab()
        {
            // return name of curtrenct active tab
            return (string)tabView.SelectedTab.Text;
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == (Key.m))
                {
                    ShowContextMenu(mousePos.X, mousePos.Y);
                    e.Handled = true;
                }
            };

            MouseClick += (e) =>
            {
                if (e.MouseEvent.Flags == contextMenu.MouseFlags)
                {
                    ShowContextMenu(e.MouseEvent.X, e.MouseEvent.Y);
                    e.Handled = true;
                }
            };
            Application.RootMouseEvent += Application_RootMouseEvent;

            void Application_RootMouseEvent(MouseEvent me)
            {
                mousePos = new Point(me.X, me.Y);
            }

            WantMousePositionReports = true;

            Application.Top.Closed += (_) =>
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Application.RootMouseEvent -= Application_RootMouseEvent;
            };
        }

        public void ShowContextMenu(int x, int y)
        {
            contextMenu = new ContextMenu(x, y,
                new MenuBarItem(new MenuItem[]
                {
                    new MenuItem ("_Configuration", "Show configuration", () => MessageBox.Query (50, 5, "Info", "This would open settings dialog", "Ok")),
                    new MenuBarItem ("More options", new MenuItem []
                    {
                        new MenuItem ("_Setup", "Change settings", () => MessageBox.Query (50, 5, "Info", "This would open setup dialog", "Ok")),
                        new MenuItem ("_Maintenance", "Maintenance mode", () => MessageBox.Query (50, 5, "Info", "This would open maintenance dialog", "Ok")),
                    }),
                        miUseSubMenusSingleFrame = new MenuItem ("Use_SubMenusSingleFrame", "",
                        () => contextMenu.UseSubMenusSingleFrame = miUseSubMenusSingleFrame.Checked = useSubMenusSingleFrame = !useSubMenusSingleFrame) {
                            CheckType = MenuItemCheckStyle.Checked, Checked = useSubMenusSingleFrame
                        },
                    null!,
                    new MenuItem ("_Quit", "", () => Application.RequestStop ())
                })
            )
            { ForceMinimumPosToZero = true, UseSubMenusSingleFrame = useSubMenusSingleFrame };

            contextMenu.Show();
        }
    }
}
