using System.Globalization;
using Terminal.Gui;
using Whale.Components;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Single
{
    public sealed class ImageWindow : Window
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerUtilityService dockerUtilityService;
        private ContextMenu contextMenu = new();
        public string ImageId { get; init; }
        public ImageWindow(string imageId) : base("Image: " + imageId)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerUtilityService = new DockerUtilityService(shellCommandRunner);
            ImageId = imageId;
            InitView();
        }

        public void InitView()
        {
            ConfigureContextMenu();

            var statusFrameView = new FrameView("Stats")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 3,
            };

            var numbers = new[] { 10, 20, 30, 40, 50 };


            var listView = new ListView(numbers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            var numbers2 = new[] { 10, 20, 30, 40, 50 };


            var listView2 = new ListView(numbers2)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            var hierarchyFrameView = new FrameView("Image hierarchy")
            {
                X = 0,
                Y = Pos.Bottom(statusFrameView),
                Width = Dim.Percent(40),
                Height = Dim.Percent(50),
            };

            hierarchyFrameView.Add(listView);


            var layersFrameView = new FrameView("Layers")
            {
                X = 0,
                Y = Pos.Bottom(hierarchyFrameView),
                Width = Dim.Percent(40),
                Height = Dim.Fill()
            };

            layersFrameView.Add(listView2);

            var infoFrameView = new FrameView("Info")
            {
                X = Pos.Right(hierarchyFrameView),
                Y = Pos.Bottom(statusFrameView),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var textView = new TextView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                ColorScheme = new ColorScheme()
                {
                    Normal = Colors.Base.Normal,
                },
            };

            infoFrameView.Add(textView);

            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker image inspect " + ImageId);
                textView.Text = result.Value.std;
            });


            Add(statusFrameView, hierarchyFrameView, layersFrameView, infoFrameView);
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
            contextMenu =
                new ContextMenu(x, y,
                    new MenuBarItem(new MenuItem[]
                    {
                        new MenuItem ("Run", "Create container", async () =>
                        {
                            var createContainerDialog = new CreateContainerDialog(ImageId);
                            createContainerDialog.ShowDialog();
                        }),
                        new MenuBarItem("Navigation", new MenuItem[]
                        {
                            new MenuItem ("Go back", "", () =>
                            {
                                ReturnToMainWindow();
                            }),
                            new MenuItem ("Quit", "", () => Application.RequestStop ()),
                        }),
                    })
                )
                { ForceMinimumPosToZero = true };

            contextMenu.Show();
        }

        public void ReturnToMainWindow()
        {
            Application.Top.RemoveAll();
            var mainWindow = MainWindow.CreateAsync();
            Dispose();
            Application.Top.Add(mainWindow);
            Application.Top.Add(MenuBarX.CreateMenuBar());
            Application.Refresh();
        }
    }
}
