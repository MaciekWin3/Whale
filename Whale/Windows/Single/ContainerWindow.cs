﻿using System.Globalization;
using Terminal.Gui;
using Whale.Components;
using Whale.Services;
using Whale.Windows.Single.ContainerTabs;

namespace Whale.Windows.Single
{
    public sealed class ContainerWindow : Window
    {
        public string ContainerId { get; init; }
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        public ContainerWindow(string containerId)
        {
            ContainerId = containerId;
            shellCommandRunner = new ShellCommandRunner();
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
            var containerInspectWindow = new ContainerInspectWindow();
            var containerTerminalWindow = new ContainerTerminalWindow(ContainerId);
            var containerFilesWindow = new ContainerFilesWindow();
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
                    case (Key.Tab):
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
                    case (Key.t):
                        OpenTerminalDialog();
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }

                if (tabView.SelectedTab.Text.ToString() == "Terminal")
                {
                    containerTerminalWindow.HandleKeyPress(e.KeyEvent);
                }
            };

            Add(tabView);


            //var goBack = new Button("Go back")
            //{
            //    X = 2,
            //    Y = 2,
            //};
            //goBack.Clicked += () =>
            //{
            //    Application.Top.RemoveAll();
            //    var mainWindow = MainWindow.CreateAsync();
            //    Application.Top.Add(mainWindow);
            //    Application.Top.Add(MenuBarX.CreateMenuBar());
            //    Application.Refresh();
            //};
            //Add(goBack);
        }

        public void OpenTerminalDialog()
        {
            var dialog = new Dialog("Terminal")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Percent(40),
                Height = Dim.Percent(40),
            };
            var label = new Label("Run command:")
            {
                X = 0,
                Y = 1,
            };
            var terminal = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(label),
                Width = Dim.Fill()
            };

            var runButton = new Button("Run");
            runButton.Clicked += async () =>
            {
                var command = terminal.Text.ToString();
                var result = await shellCommandRunner.RunCommandAsync(command);
                if (result.IsFailure)
                {
                    MessageBox.ErrorQuery(50, 7, "Error", result.Error, "Ok");
                }
                else
                {
                    MessageBox.Query(50, 7, "Info", result.Value.std, "Ok");
                }
            };

            var exitButton = new Button("Exit");
            exitButton.Clicked += () =>
            {
                Application.RequestStop();
            };

            dialog.Add(label, terminal);
            dialog.AddButton(runButton);
            dialog.AddButton(exitButton);
            Application.Run(dialog);
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key is (Key.M | Key.CtrlMask) || e.KeyEvent.Key is (Key.m))
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
            contextMenu = new ContextMenu(x, y,
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
