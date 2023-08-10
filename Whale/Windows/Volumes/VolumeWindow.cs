using System.Runtime.InteropServices;
using Terminal.Gui;
using Terminal.Gui.Trees;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils.Helpers;
using Whale.Windows.Containers;

namespace Whale.Windows.Volumes
{
    public sealed class VolumeWindow : Window
    {
        public string VolumeId { get; init; }
        List<Container> Containers { get; set; } = new();
        ListView list = new();
        public string VolumeInfo { get; set; } = string.Empty;
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerVolumeService dockerVolumeService;
        TreeView<FileSystemInfo> treeViewFiles = new();
        FrameView configView = new();
        TextView textView = new();
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

            textView = new TextView()
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

            configView.Add(textView);

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
                    Application.Top.Add(new Navbar());
                }
            };

            used.Add(list);

            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker volume inspect " + VolumeId);
                if (result.IsSuccess)
                {
                    textView.Text = result.Value.std;
                    VolumeInfo = result.Value.std;
                }
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
            if (File.Exists(e.NewValue.FullName))
            {
                configView.Title = e.NewValue.Name;
                textView.Text = File.ReadAllText(e.NewValue.FullName);
            }
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
        }

        public void ShowContextMenu(int x, int y)
        {
            contextMenu =
                new ContextMenu(x, y,
                    new MenuBarItem(new MenuItem[]
                    {
                        new MenuItem ("Inspect", "Volume info", () =>
                        {
                            configView.Text = VolumeInfo;
                            configView.Title = "Details";
                        }),
                        new MenuItem ("Delete", "Delete volume", async () =>
                        {
                            await shellCommandRunner.RunCommandAsync("docker volume rm " + VolumeId);
                            NavigationHelper.ReturnToMainWindow("Volumes");
                        }),
                        null!,
                        new MenuItem ("Back", "", () =>
                        {
                            NavigationHelper.ReturnToMainWindow("Volumes");
                        }),
                        new MenuItem ("Quit", "", () => Application.RequestStop ()),
                    })
                );
            contextMenu.Show();
        }
    }
}
