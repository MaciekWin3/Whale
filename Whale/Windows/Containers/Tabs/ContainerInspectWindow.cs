﻿using Terminal.Gui;
using Whale.Components;

namespace Whale.Windows.Containers.Tabs
{
    public sealed class ContainerInspectWindow : Toplevel
    {
        public ContainerInspectWindow() : base()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            ColorScheme = Colors.Base;
        }
        public void InitView()
        {
            var label = new Label("Test Window")
            {
                X = 5,
                Y = 5,
            };
            Add(label);

            var goBack = new Button("Go back")
            {
                X = 6,
                Y = Pos.Bottom(label),
            };
            goBack.Clicked += () =>
            {
                Application.Top.RemoveAll();
                var mainWindow = MainWindow.CreateAsync();
                Application.Top.Add(mainWindow);
                Application.Top.Add(MenuBarX.CreateMenuBar());
                Application.Refresh();
            };
            Add(goBack);
        }
    }
}