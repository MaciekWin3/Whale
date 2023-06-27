﻿using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public class ContainerListWindow : Window
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public List<string> ContainerList { get; set; } = new();

        private readonly Dictionary<string, Delegate> events = new();
        private readonly MainWindow mainWindow;
        ColorScheme alternatingColorScheme = null!;
        public ContainerListWindow(Dictionary<string, Delegate> events, MainWindow mainWindow)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
            this.events = events;
            this.mainWindow = mainWindow;
        }

        public void InitView()
        {
            var tableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };

            alternatingColorScheme = new ColorScheme()
            {

                Disabled = ColorScheme.Disabled,
                HotFocus = ColorScheme.HotFocus,
                Focus = ColorScheme.Focus,
                Normal = Application.Driver.MakeAttribute(Color.Magenta, Color.BrightGreen)
            };

            tableView.Style.SmoothHorizontalScrolling = true;
            tableView.Style.ExpandLastColumn = true;
            //tableView.Style.RowColorGetter = (a) => { return a.RowIndex % 2 == 0 ? alternatingColorScheme : null; };

            tableView.CellActivated += (e) =>
            {
                int row = e.Row;
                var name = (string)e.Table.Rows[row][0];
                if (name is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(name);
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<Container>> cache = Result.Fail<List<Container>>("Initial cache value");
                while (true)
                {
                    Result<List<Container>> result = await dockerService.GetContainerListAsync();
                    if (mainWindow.GetSelectedTab() is "Containers" && result.IsSuccess)
                    {
                        bool? isSame = cache?.Value?.SequenceEqual(result.GetValue());
                        bool? isCacheSuccessful = cache?.IsSuccess;
                        if (isSame == true && isCacheSuccessful == true)
                        {
                            continue;
                        }
                        else
                        {
                            tableView.Table = ConvertListToDataTable(result.GetValue());
                            cache = result;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            });

            KeyDown += (e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    events["ShowContextMenu"].DynamicInvoke(1, 1);
                }
            };

            Add(tableView);
        }

        public static DataTable ConvertListToDataTable(List<Container> list)
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Image", typeof(string));
            table.Columns.Add("Command", typeof(string));
            //table.Columns.Add("Created", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Ports", typeof(string));
            table.Columns.Add("Names", typeof(string));

            foreach (var item in list)
            {
                //table.Rows.Add(item.ID, item.Image, item.Command, item.CreatedAt, item.Status, item.Ports, item.Names);
                table.Rows.Add(item.ID, item.Image, item.Command, item.Status, item.Ports, item.Names);
            }
            return table;
        }


    }
}
