using System.Globalization;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Single
{
    public sealed class VolumeWindow : Window
    {
        public string VolumeId { get; init; }
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerVolumeService dockerVolumeService;
        public VolumeWindow(string volumeId) : base("Image")
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerVolumeService = new DockerVolumeService(shellCommandRunner);
            VolumeId = volumeId;
            InitView();
        }
        public void InitView()
        {
            ConfigureContextMenu();

            var files = new FrameView()
            {
                X = 1,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Title = "Files"
                },
                Text = "Here should be files"
            };

            var used = new FrameView()
            {
                X = 1,
                Y = Pos.Bottom(files),
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Title = "Containers"
                },
            };

            var configView = new FrameView()
            {
                X = Pos.Right(files),
                Y = 0,
                Width = Dim.Percent(50) - 1,
                Height = Dim.Fill(),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Title = "Config"
                },
                Text = "Here should be config"
            };

            List<Container> containers = new();

            var list = new ListView(containers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };

            // When user selects a container, show its details
            list.OpenSelectedItem += (e) =>
            {
                if (e is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(e.Value.ToString());
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };

            used.Add(list);

            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker volume inspect " + VolumeId);
                configView.Text = result.Value.std;
            });

            Application.MainLoop.Invoke(async () =>
            {
                var result = await dockerVolumeService.GetVolumesContainersListAsync(VolumeId);
                list.SetSource(result?.Value?.Select(c => $"{c.Names}").ToList());
            });

            Add(files, used, configView);
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key is (Key.M | Key.CtrlMask) || e.KeyEvent.Key is (Key.m))
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
                        new MenuItem ("Delete", "Delete volume", async () =>
                        {
                            await shellCommandRunner.RunCommandAsync("docker volume rm " + VolumeId);
                            ReturnToMainWindow();
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
