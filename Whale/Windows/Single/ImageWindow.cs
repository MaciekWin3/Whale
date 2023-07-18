using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Single
{
    public sealed class ImageWindow : Window
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerImageService dockerImageService;
        private ContextMenu contextMenu = new();
        public string ImageId { get; init; }
        public ImageWindow(string imageId) : base("Image: " + imageId)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerImageService = new DockerImageService(shellCommandRunner);
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

            var layers = new List<ImageLayer>();

            var listView = new ListView(layers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            var layerslistView = new ListView(layers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            var layersFrameView = new FrameView("Layers")
            {
                X = 0,
                //Y = Pos.Bottom(hierarchyFrameView),
                Y = Pos.Bottom(statusFrameView),
                Width = Dim.Percent(40),
                Height = Dim.Fill()
            };

            layersFrameView.Add(layerslistView);

            var infoFrameView = new FrameView("Config")
            {
                X = Pos.Right(layersFrameView),
                Y = Pos.Bottom(statusFrameView),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var scroll = new ScrollView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                ColorScheme = Colors.Menu,
            };

            infoFrameView.Add(scroll);
            scroll.Add(textView);

            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker image inspect " + ImageId);
                infoFrameView.Text = result.Value.std;
                textView.Text = result.Value.std;
            });


            Application.MainLoop.Invoke(async () =>
            {
                var imageLayers = await dockerImageService.GetImageLayersAsync(ImageId);
                layerslistView.SetSource(imageLayers?.Value?.Select((layer, i) => $"{i + 1} {layer.CreatedBy} {layer.Size}").ToList());
            });

            Add(statusFrameView, layersFrameView, infoFrameView);
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
        }

        public void ShowContextMenu(int x, int y)
        {
            contextMenu =
                new ContextMenu(x, y,
                    new MenuBarItem(new MenuItem[]
                    {
                        new MenuItem ("Run", "Create container", () =>
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
            Application.Top.Add(mainWindow);
            Application.Top.Add(MenuBarX.CreateMenuBar());
            Application.Refresh();
        }
    }
}
