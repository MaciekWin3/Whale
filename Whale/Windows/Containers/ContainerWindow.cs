using System.Globalization;
using Terminal.Gui;
using Whale.Components;
using Whale.DTOs.Container;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Windows.Containers.Tabs;

namespace Whale.Windows.Containers
{
    public sealed class ContainerWindow : Window
    {
        public string ContainerId { get; init; }
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerUtilityService dockerUtilityService;
        public ContainerWindow(string containerId)
        {
            ContainerId = containerId;
            shellCommandRunner = new ShellCommandRunner();
            dockerUtilityService = new DockerUtilityService(shellCommandRunner);
            InitView();
            var color = Color.Green;
            Border = new Border
            {
                Title = "Container: " + containerId,
                BorderBrush = color,
                BorderStyle = BorderStyle.Rounded,
            };
        }

        public void InitView()
        {
            ConfigureContextMenu();
            var tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Fill(),
            };

            var containerLogsWindow = new ContainerLogsWindow(ContainerId);
            var containerInspectWindow = new ContainerInspectWindow(ContainerId);
            var containerTerminalWindow = new ContainerTerminalWindow(ContainerId);
            var containerFilesWindow = new ContainerFilesWindow(ContainerId);
            var containerStatsWindow = new ContainerStatsWindow(ContainerId);

            tabView.AddTab(new TabView.Tab("Logs", containerLogsWindow), false);
            tabView.AddTab(new TabView.Tab("Inspect", containerInspectWindow), false);
            tabView.AddTab(new TabView.Tab("Terminal", containerTerminalWindow), false);
            tabView.AddTab(new TabView.Tab("Files", containerFilesWindow), false);
            tabView.AddTab(new TabView.Tab("Stats", containerStatsWindow), false);

            KeyPress += (e) =>
            {
                switch (e.KeyEvent.Key)
                {
                    case Key.Tab:
                        var tabs = tabView.Tabs.Count;
                        if (tabView.SelectedTab == tabView.Tabs.ToArray()[tabs - 1])
                        {
                            tabView.SelectedTab = tabView.Tabs.ToArray()[0];
                        }
                        else
                        {
                            tabView.SwitchTabBy(1);
                        }
                        e.Handled = true;
                        break;
                    case Key.t:
                        OpenTerminalDialog();
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }

                switch (tabView.SelectedTab.Text.ToString())
                {
                    case "Terminal":
                        containerTerminalWindow.HandleKeyPress(e.KeyEvent);
                        break;
                    case "Logs":
                        e.Handled = true;
                        break;
                    case "Inspect":
                        e.Handled = true;
                        break;
                    case "Files":
                        e.Handled = true;
                        break;
                    case "Stats":
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            };


            // TODO: Fix this (performance)
            //Application.MainLoop.Invoke(async () =>
            //{
            //    while (true)
            //    {
            //        var containerState = await GetContainerState();
            //        Border.BorderBrush = containerState switch
            //        {
            //            ContainerState.Running => Color.Green,
            //            ContainerState.Paused => Color.BrightYellow,
            //            ContainerState.Created => Color.Blue,
            //            ContainerState.Restarting => Color.BrightBlue,
            //            ContainerState.Removing => Color.DarkGray,
            //            ContainerState.Exited => Color.Red,
            //            ContainerState.Dead => Color.Black,
            //            _ => Color.Gray
            //        };
            //    }
            //});

            Add(tabView);
        }

        public async Task<ContainerState> GetContainerState()
        {
            var result = await dockerUtilityService.GetDockerObjectInfoAsync<ContainerDTO>(ContainerId);
            if (result.GetValue() is not null && result?.Value?.State is not null)
            {
                var state = result.Value.State.Status;

                if (state is null)
                {
                    return ContainerState.Unknown;
                }

                return state switch
                {
                    var s when s.Contains("running") => ContainerState.Running,
                    var s when s.Contains("paused") => ContainerState.Paused,
                    var s when s.Contains("created") => ContainerState.Created,
                    var s when s.Contains("restarting") => ContainerState.Restarting,
                    var s when s.Contains("removing") => ContainerState.Removing,
                    var s when s.Contains("exited") => ContainerState.Exited,
                    var s when s.Contains("dead") => ContainerState.Dead,
                    _ => ContainerState.Unknown
                };
            }
            return ContainerState.Unknown;
        }

        public static void OpenTerminalDialog()
        {
            var terminal = new TerminalDialog();
            terminal.ShowDialog();
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key is (Key.M | Key.CtrlMask) || e.KeyEvent.Key is Key.m)
                {
                    ShowContextMenu(mousePos.X, mousePos.Y);
                }
                e.Handled = true;
            };

            MouseClick += (e) =>
            {
                if (e.MouseEvent.Flags == contextMenu.MouseFlags)
                {
                    ShowContextMenu(e.MouseEvent.X, e.MouseEvent.Y);
                }
                e.Handled = true;
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
                        new MenuItem ("Run", "Run container", async () =>
                        {
                            await shellCommandRunner.RunCommandAsync($"docker start {ContainerId}");

                        }),
                        new MenuItem ("Pause", "Pause container", async () =>
                        {
                            await shellCommandRunner.RunCommandAsync($"docker pause {ContainerId}");
                        }),
                        new MenuItem("Delete", "Delete container", async () =>
                        {
                            await shellCommandRunner.RunCommandAsync($"docker rm {ContainerId}");
                            ReturnToMainWindow();
                        }),
                        null!,
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
                {
                    ForceMinimumPosToZero = true
                };

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
