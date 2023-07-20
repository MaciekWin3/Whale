using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Images
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

            var textView = new TextView()
            {
                X = Pos.Right(layersFrameView),
                Y = Pos.Bottom(statusFrameView),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
            };

            Add(textView);

            var scrollBar = new ScrollBarView(textView, true, true);

            scrollBar.ChangedPosition += () =>
            {
                textView.TopRow = scrollBar.Position;
                if (textView.TopRow != scrollBar.Position)
                {
                    scrollBar.Position = textView.TopRow;
                }
                textView.SetNeedsDisplay();
            };

            scrollBar.OtherScrollBarView.ChangedPosition += () =>
            {
                textView.LeftColumn = scrollBar.OtherScrollBarView.Position;
                if (textView.LeftColumn != scrollBar.OtherScrollBarView.Position)
                {
                    scrollBar.OtherScrollBarView.Position = textView.LeftColumn;
                }
                textView.SetNeedsDisplay();
            };

            scrollBar.VisibleChanged += () =>
            {
                if (scrollBar.Visible && textView.RightOffset == 0)
                {
                    textView.RightOffset = 1;
                }
                else if (!scrollBar.Visible && textView.RightOffset == 1)
                {
                    textView.RightOffset = 0;
                }
            };

            scrollBar.OtherScrollBarView.VisibleChanged += () =>
            {
                if (scrollBar.OtherScrollBarView.Visible && textView.BottomOffset == 0)
                {
                    textView.BottomOffset = 1;
                }
                else if (!scrollBar.OtherScrollBarView.Visible && textView.BottomOffset == 1)
                {
                    textView.BottomOffset = 0;
                }
            };

            textView.DrawContent += (e) =>
            {
                scrollBar.Size = textView.Lines;
                scrollBar.Position = textView.TopRow;
                if (scrollBar.OtherScrollBarView != null)
                {
                    scrollBar.OtherScrollBarView.Size = textView.Maxlength;
                    scrollBar.OtherScrollBarView.Position = textView.LeftColumn;
                }
                scrollBar.LayoutSubviews();
                scrollBar.Refresh();
            };


            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker image inspect " + ImageId);
                textView.Text = result.Value.std;
            });


            Application.MainLoop.Invoke(async () =>
            {
                var imageLayers = await dockerImageService.GetImageLayersAsync(ImageId);
                layerslistView.SetSource(imageLayers?.Value?.Select((layer, i) => $"{i + 1} {layer.CreatedBy} {layer.Size}").ToList());
            });

            Add(statusFrameView, layersFrameView);
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.m)
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
