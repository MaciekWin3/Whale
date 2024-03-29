﻿using Terminal.Gui;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils.Helpers;
using Whale.Windows.Images.Components;

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

            var layers = new List<ImageLayer>();

            var layerslistView = new ListView(layers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            var layersFrameView = new FrameView("Layers")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(40),
                Height = Dim.Fill()
            };

            layersFrameView.Add(layerslistView);

            var frameView = new FrameView("Config")
            {
                X = Pos.Right(layersFrameView),
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                WordWrap = true,
                ReadOnly = true,
                ColorScheme = new ColorScheme()
                {
                    Focus = new Terminal.Gui.Attribute(Color.White, Color.Blue),
                },
            };

            frameView.Add(textView);

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

            Add(frameView, layersFrameView);
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key is (Key.M | Key.CtrlMask) || e.KeyEvent.Key is Key.m)
                {
                    ShowContextMenu(mousePos.X, mousePos.Y);
                    e.Handled = true;
                }
                if (e.KeyEvent.Key is (Key.B | Key.CtrlMask) || e.KeyEvent.Key is Key.b)
                {
                    NavigationHelper.ReturnToMainWindow("Containers");
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
                        null!,
                        new MenuItem ("Back", "", () =>
                        {
                            NavigationHelper.ReturnToMainWindow("Images");
                        }),
                        new MenuItem ("Quit", "Quit app", () => Application.RequestStop ()),
                    })
                );
            contextMenu.Show();
        }
    }
}
