using System.Globalization;
using System.Runtime.InteropServices;
using Terminal.Gui;
using Terminal.Gui.Trees;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Single
{
    public sealed class VolumeWindow : Window
    {
        public string VolumeId { get; init; }
        List<Container> Containers { get; set; } = new();
        ListView list = new();
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerVolumeService dockerVolumeService;
        TreeView<FileSystemInfo> treeViewFiles = new();
        FrameView configView = new();
        public VolumeWindow(string volumeId) : base("Volume")
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerVolumeService = new DockerVolumeService(shellCommandRunner);
            VolumeId = volumeId;
            InitView();
        }
        public void InitView()
        {
            ConfigureContextMenu();

            treeViewFiles = new TreeView<FileSystemInfo>()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
            };

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
            };
            files.Add(treeViewFiles);

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
                CanFocus = false
            };

            configView = new FrameView()
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
            };

            list = new ListView(Containers)
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                CanFocus = false
            };

            list.OpenSelectedItem += (e) =>
            {
                var containerId = e.Value as string;
                if (containerId is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(containerId);
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
                if (result?.Value?.Count > 0)
                {
                    used.CanFocus = true;
                    list.CanFocus = true;
                }
            });

            SetupFileTree();
            treeViewFiles.MouseClick += TreeViewFilesMouseClick;
            treeViewFiles.KeyPress += TreeViewFilesKeyPress;
            treeViewFiles.SelectionChanged += TreeViewFilesSelectionChanged;

            Add(files, used, configView);
        }

        public async Task GetVolumesContainersListAsync()
        {
            var result = await dockerVolumeService.GetVolumesContainersListAsync(VolumeId);
            list.SetSource(result?.Value?.Select(c => $"{c.Names}").ToList());
        }

        private void TreeViewFilesSelectionChanged(object? sender, SelectionChangedEventArgs<FileSystemInfo> e)
        {
            configView.Text = e.NewValue.FullName;
        }

        private void TreeViewFilesKeyPress(KeyEventEventArgs args)
        {
            return;
        }
        private void TreeViewFilesMouseClick(MouseEventArgs args)
        {
            return;
        }

        private void SetupFileTree()
        {
            treeViewFiles.TreeBuilder = new DelegateTreeBuilder<FileSystemInfo>(GetChildren, (o) => o is DirectoryInfo);
            treeViewFiles.AspectGetter = FileSystemAspectGetter;
            treeViewFiles.AddObject(GetDockerVolumeDirectoryInfo(VolumeId));
        }

        private DirectoryInfo GetDockerVolumeDirectoryInfo(string volumeName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new DirectoryInfo($@"\\wsl$\docker-desktop-data\data\docker\volumes\{volumeName}\_data\");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new DirectoryInfo($"/var/lib/docker/volumes/{volumeName}/_data/");
            }
            else
            {
                throw new NotSupportedException("Unsupported operating system.");
            }
        }

        private string FileSystemAspectGetter(FileSystemInfo model)
        {
            if (model is DirectoryInfo d)
            {
                return d.Name;
            }
            if (model is FileInfo f)
            {
                return f.Name;
            }

            return model.ToString();
        }

        private IEnumerable<FileSystemInfo> GetChildren(FileSystemInfo model)
        {
            if (model is DirectoryInfo d)
            {
                try
                {
                    return d.GetFileSystemInfos()
                        .OrderBy(a => a is DirectoryInfo ? 0 : 1)
                        .ThenBy(b => b.Name);
                }
                catch (SystemException)
                {
                    return Enumerable.Empty<FileSystemInfo>();
                }
            }

            return Enumerable.Empty<FileSystemInfo>(); ;
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
            Application.Top.Add(mainWindow);
            Application.Top.Add(MenuBarX.CreateMenuBar());
            Application.Refresh();
        }
    }
}
