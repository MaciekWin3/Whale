﻿using System.Globalization;
using System.Text.Json;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;
using Whale.Objects.Container;
using Whale.Services;
using Whale.Windows.Lists;

namespace Whale.Windows
{
    public class MainWindow : Window
    {
        GraphView graphView = null!;
        private ContextMenu contextMenu = new();
        private MenuItem miUseSubMenusSingleFrame = null!;
        private bool useSubMenusSingleFrame;
        private readonly ShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        private readonly Dictionary<string, Delegate> events;

        public TextView DetailsText { get; set; }
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
                { nameof(ChangeText), new Func<string, Task>(ChangeText) },
            };
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
                Width = Dim.Percent(30),
                Height = Dim.Fill(),
            };

            // Tabs
            //tabView.AddTab(new TabView.Tab("Chart", Bar()), false);
            var containerWindow = new ContainerListWindow(events);
            var imageWindow = new ImageListWindow(ShowContextMenu);
            var volumeWindow = new VolumeListWindow(ShowContextMenu);

            tabView.AddTab(new TabView.Tab("Containers", containerWindow), false);
            tabView.AddTab(new TabView.Tab("Volumes", volumeWindow), false);

            tabView.SelectedTabChanged += (a, e) =>
            {
                if (e.NewTab.Text == "Containers")
                {
                    DetailsText.Text = "Loading...";
                    Application.MainLoop.Invoke(async () =>
                    {
                        var z = containerWindow.GetCurrnetContainerName();
                        var x = await shellCommandRunner.RunCommandAsync("docker", "inspect", z);
                    });
                }
                else if (e.NewTab.Text == "Images")
                {
                }
                else if (e.NewTab.Text == "Volumes")
                {
                    Application.Top.RemoveAll();
                    Application.Top.Add(imageWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };
            tabView.Style.ShowBorder = true;
            tabView.ApplyStyleChanges();

            Add(tabView);

            var scrollView = new ScrollView
            {
                X = Pos.Right(tabView),
                Y = 0,
                Width = Dim.Percent(70),
                Height = Dim.Percent(100),
                ContentSize = new Size(500, 500),
                ShowVerticalScrollIndicator = true,
                ShowHorizontalScrollIndicator = true,
            };

            DetailsText = new TextView()
            {
                X = 1,
                Y = 1,
                AutoSize = true,
                ColorScheme = Colors.Dialog,
                Width = Dim.Percent(50) - 1,
                Height = Dim.Percent(50) - 1,
            };

            scrollView.Add(DetailsText);
            Add(scrollView);

            Application.MainLoop.Invoke(async () =>
            {
                var y = await shellCommandRunner.RunCommandAsync("cmd", "/C", "echo", "TEst");
                DetailsText.Text = y.Value.std.Trim();
            });
        }

        public async Task ChangeText(string text)
        {
            var d = await dockerService.GetDockerObjectInfoAsync<Container>(text);
            if (d.Value is not null)
            {
                var jsonText = JsonSerializer.Serialize(d.Value, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
                DetailsText.Text = jsonText;
            }
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
