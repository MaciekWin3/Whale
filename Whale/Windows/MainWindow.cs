using CliWrap;
using CliWrap.EventStream;
using System.Globalization;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;
using Whale.Services;

namespace Whale.Windows
{
    public class MainWindow : Window
    {
        GraphView graphView = null!;
        private ContextMenu contextMenu = new();
        private bool forceMinimumPosToZero = true;
        private MenuItem miUseSubMenusSingleFrame = null!;
        private bool useSubMenusSingleFrame;
        private ShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        protected MainWindow() : base("Whale Dashboard")
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

        public void InitWindow()
        {
            ConfigureContextMenu();

            var tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill(),

            };

            var containersFrame = new FrameView("Containers")
            {
                X = Pos.Right(tabView),
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(35),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Effect3D = false,
                    Title = "Containers"
                }
            };

            var imagesFrame = new FrameView("Images")
            {
                X = Pos.Right(tabView),
                Y = Pos.Bottom(containersFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(35),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Effect3D = false,
                    Title = "Containers"
                }
            };

            var volumesFrame = new FrameView("Volumes")
            {
                X = Pos.Right(tabView),
                Y = Pos.Bottom(imagesFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(35),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Effect3D = false,
                    Title = "Containers"
                }
            };

            var textContainers = new Label()
            {
                Text = "Loading...",
            };

            var textImages = new Label()
            {
                Text = "Loading..."
            };

            var textVolumes = new Label()
            {
                Text = "Loading..."
            };

            imagesFrame.Add(textImages);
            Add(imagesFrame);
            containersFrame.Add(textContainers);
            Add(containersFrame);
            volumesFrame.Add(textVolumes);
            Add(volumesFrame);


            Application.MainLoop.Invoke(async () =>
            {
                var x = await shellCommandRunner.RunCommandAsync("docker", "image", "ls");
                textImages.Text = x.Value.std;
                //var y = await ShellCommandRunner.RunCommandAsync("docker", "container", "ls");
                var z = await shellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                textVolumes.Text = z.Value.std;
                //Application.Refresh();
            });

            Application.MainLoop.Invoke(async () =>
            {
                var cmd = Cli.Wrap("ping").WithArguments("wp.pl");
                await foreach (var cmdEvent in cmd.ListenAsync())
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            //_output.WriteLine($"Process started; ID: {started.ProcessId}");
                            textVolumes.Clear();
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            textVolumes.Text += $"{stdOut.Text}\n";
                            break;
                        case StandardErrorCommandEvent stdErr:
                            //_output.WriteLine($"Err> {stdErr.Text}");
                            break;
                        case ExitedCommandEvent exited:
                            //_output.WriteLine($"Process exited; Code: {exited.ExitCode}");
                            break;
                    }
                }
                //Application.Refresh();
            });


            // Tabs
            //tabView.AddTab(new TabView.Tab("Chart", Bar()), false);
            tabView.AddTab(new TabView.Tab("Containers", new ContainerWindow()), false);
            tabView.AddTab(new TabView.Tab("Images", new ImageWindow(ShowContextMenu)), false);
            tabView.AddTab(new TabView.Tab("Volumes", new VolumeWindow(ShowContextMenu)), false);
            // write me a code that will capture event of swithc tabs and refresh data
            tabView.SelectedTabChanged += (a, e) =>
            {
                if (e.NewTab.Text == "Containers")
                {
                    textContainers.Text = "Loading...";
                    Application.MainLoop.Invoke(async () =>
                    {
                        var x = await shellCommandRunner.RunCommandAsync("docker", "container", "ls");
                        textContainers.Text = x.Value.std;
                    });
                }
                else if (e.NewTab.Text == "Images")
                {
                    textImages.Text = "Loading...";
                    Application.MainLoop.Invoke(async () =>
                    {
                        var x = await shellCommandRunner.RunCommandAsync("docker", "image", "ls");
                        textImages.Text = x.Value.std;
                    });
                }
                else if (e.NewTab.Text == "Volumes")
                {
                    textVolumes.Text = "Loading...";
                    Application.MainLoop.Invoke(async () =>
                    {
                        //var x = await ShellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                        //textVolumes.Text = x.Value.std;
                        var x = "Fun word";
                        textVolumes.Text = x;
                    });
                }
            };
            tabView.Style.ShowBorder = true;
            tabView.ApplyStyleChanges();

            Add(tabView);
        }

        private View Bar()
        {
            var imagesView = new View()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            graphView = new GraphView()
            {
                X = 1,
                Y = 1,
                Width = 60,
                Height = 20,
            };
            SetupDisco();
            imagesView.Add(graphView);
            return imagesView;
        }

        private void SetupDisco()
        {
            graphView.Reset();

            graphView.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

            var stiple = new GraphCellToRender('\u2593');

            Random r = new();
            var series = new DiscoBarSeries();
            var bars = new List<BarSeries.Bar>();
            for (int i = 0; i < 31; i++)
            {
                bars.Add(new BarSeries.Bar(null, stiple, 1));
            }

            Func<MainLoop, bool> genSample = (l) =>
            {
                bars.RemoveAt(0);
                //Random random = new Random();
                //int randomNumber = random.Next(1, 101);
                int randomNumber = 10;
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView.Series.Contains(series);
            };

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), genSample);

            series.Bars = bars;

            graphView.Series.Add(series);

            // How much graph space each cell of the console depicts
            graphView.CellSize = new PointF(1, 10);
            graphView.AxisX.Increment = 0; // No graph ticks
            graphView.AxisX.ShowLabelsEvery = 0; // no labels

            graphView.AxisX.Visible = false;
            graphView.AxisY.Visible = false;

            graphView.SetNeedsDisplay();
        }

        private void ShowContextMenu(int x, int y)
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
            { ForceMinimumPosToZero = forceMinimumPosToZero, UseSubMenusSingleFrame = useSubMenusSingleFrame };

            contextMenu.Show();
        }
    }
}
