using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Windows.Containers
{
    public sealed class ContainerListWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        public List<string> ContainerList { get; set; } = new();
        private bool showAll = true;

        private readonly MainWindow mainWindow;
        private ColorScheme alternatingColorScheme = null!;
        private ContextMenu contextMenu = new();
        TableView tableView = null!;

        public ContainerListWindow(MainWindow mainWindow)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerContainerService = new DockerContainerService(shellCommandRunner);

            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            this.mainWindow = mainWindow;
            ColorScheme = Colors.Base;
        }

        public void InitView()
        {
            tableView = new TableView()
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
                HotFocus = Application.Driver.MakeAttribute(Color.Green, Color.White),
                Focus = ColorScheme.Focus,
                Normal = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Blue)
            };

            tableView.Style.SmoothHorizontalScrolling = true;
            tableView.Style.ExpandLastColumn = true;
            tableView.Style.RowColorGetter = (colorGetter) =>
            {
                var rowIndex = colorGetter.RowIndex;
                int containerStatusColumnNumber = 3;
                DataRowCollection rows = colorGetter.Table.Rows;
                var containerStatus = rows[rowIndex][containerStatusColumnNumber].ToString();

                bool isContainerUp = containerStatus?.Contains("Up") ?? false;

                if (isContainerUp)
                {
                    return alternatingColorScheme;
                }
                return null;
            };

            tableView.KeyPress += TableKeyPress;
            tableView.MouseClick += TableViewMouseClick;

            tableView.CellActivated += (e) =>
            {
                int row = e.Row;
                var id = (string)e.Table.Rows[row][0];
                if (id is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(id);
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(new Navbar());
                    Application.Top.Add(new AppInfoBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<Container>> cache = Result.Fail<List<Container>>("Initial cache value");
                while (true)
                {
                    Result<List<Container>> result = await dockerContainerService.GetContainerListAsync(showAll);
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

            Add(tableView);
        }

        public static DataTable ConvertListToDataTable(List<Container> list)
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Image", typeof(string));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Ports", typeof(string));
            table.Columns.Add("Names", typeof(string));

            foreach (var item in list)
            {
                table.Rows.Add(item.ID, item.Image, item.Command, item.Status, item.Ports, item.Names);
            }
            return table;
        }

        private void TableViewMouseClick(MouseEventArgs obj)
        {
            if (obj.MouseEvent.Flags.HasFlag(MouseFlags.Button3Clicked))
            {
                tableView.SetSelection(1, obj.MouseEvent.Y - 3, false);
                try
                {
                    var id = (string)tableView.Table.Rows[obj.MouseEvent.Y - 3][0];
                    ShowContextMenu(new Point(
                        obj.MouseEvent.X + tableView.Frame.X + 5,
                        obj.MouseEvent.Y + tableView.Frame.Y + 5),
                        id);
                }
                catch
                {
                }
            }
        }

        private void TableKeyPress(KeyEventEventArgs obj)
        {
            if (obj.KeyEvent.Key == (Key.R | Key.CtrlMask))
            {
                var selected = tableView.SelectedRow;
                var id = (string)tableView.Table.Rows[selected][0];
                obj.Handled = true;

                ShowContextMenu(new Point(
                    1,
                    tableView.SelectedRow + 5),
                    id);
            }
        }

        public void ShowContextMenu(Point screenPoint, string containerName)
        {
            contextMenu =
                new ContextMenu(screenPoint.X, screenPoint.Y,
                    new MenuBarItem(new MenuItem[]
                    {
                        new MenuItem ("Inspect", "Inspect container", () =>
                        {
                            if (containerName is not null)
                            {
                                Application.Top.RemoveAll();
                                var containerWindow = new ContainerWindow(containerName);
                                Application.Top.Add(containerWindow);
                                Application.Top.Add(new Navbar());
                                Application.Top.Add(new AppInfoBar());
                            }
                        }),
                        new MenuItem ("Start", "Start container", async () =>
                        {
                            await dockerContainerService.StartContainerAsync(containerName);

                        }),
                        new MenuItem ("Pause/Unpause", "Pause/Unpause container", async () =>
                        {
                            // Handle pause/unpause
                            await dockerContainerService.PauseContainerAsync(containerName);
                        }),
                        new MenuItem("Delete", "Delete container", async () =>
                        {
                            await dockerContainerService.DeleteContainerAsync(containerName);
                        }),
                        null!,
                        new MenuItem("Show All", "", () => showAll = !showAll)
                        {
                            Checked = showAll,
                            CheckType = MenuItemCheckStyle.Checked
                        },
                        null!,
                        new MenuItem("Exit", "Exit", () =>
                        {
                            Application.RequestStop();
                        })
                    })
                )
                {
                    ForceMinimumPosToZero = true
                };
            contextMenu.Show();
        }
    }
}