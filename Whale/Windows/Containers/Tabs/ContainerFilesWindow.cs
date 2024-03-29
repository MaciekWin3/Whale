﻿using Terminal.Gui;

namespace Whale.Windows.Containers.Tabs
{
    public sealed class ContainerFilesWindow : Window
    {
        public string ContainerId { get; set; }
        public ContainerFilesWindow(string containerId)
        {
            ContainerId = containerId;
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            ColorScheme = Colors.Base;
        }

        public void InitView()
        {
            // File sysytem exploer
            var treeView = new TreeView
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
            };

            Add(treeView);
        }
    }
}
