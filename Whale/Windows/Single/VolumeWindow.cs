﻿using System.Globalization;
using Terminal.Gui;
using Whale.Components;
using Whale.Services;

namespace Whale.Windows.Single
{
    public sealed class VolumeWindow : Window
    {
        public string VolumeId { get; init; }
        private ContextMenu contextMenu = new();
        private readonly IShellCommandRunner shellCommandRunner;
        public VolumeWindow(string volumeId) : base("Image")
        {
            shellCommandRunner = new ShellCommandRunner();
            VolumeId = volumeId;
            InitView();
        }
        public void InitView()
        {
            ConfigureContextMenu();

            var label = new Label("Volume ID: " + "Here should be name")
            {
                X = 1,
                Y = 0,
                Width = Dim.Percent(40),
                Height = 2
            };

            var frameView = new FrameView()
            {
                X = Pos.Right(label),
                Y = 0,
                Width = Dim.Percent(60) - 1,
                Height = Dim.Fill(),
                Border = new Border
                {
                    BorderStyle = BorderStyle.Rounded,
                    Title = "Config"
                },
                Text = "Here should be config"
            };

            Application.MainLoop.Invoke(async () =>
            {
                var result = await shellCommandRunner.RunCommandAsync("docker volume inspect " + VolumeId);
                frameView.Text = result.Value.std;
            });

            Add(label, frameView);
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
