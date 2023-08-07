using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;
using Whale.Windows.Images;

namespace Whale.Windows.Volumes
{
    public sealed class VolumeListWindow : Toplevel
    {
        private readonly MainWindow mainWindow;
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerVolumeService dockerVolumeService;
        TableView tableView = null!;
        private ContextMenu contextMenu = new();
        public VolumeListWindow(MainWindow mainWindow)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerVolumeService = new DockerVolumeService(shellCommandRunner);
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border()
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

            tableView.KeyPress += TableKeyPress;
            tableView.MouseClick += TableViewMouseClick;

            Add(tableView);

            tableView.CellActivated += (e) =>
            {
                int row = e.Row;
                var name = (string)e.Table.Rows[row][0];
                if (name is not null)
                {
                    Application.Top.RemoveAll();
                    var volumeWindow = new VolumeWindow(name);
                    Application.Top.Add(volumeWindow);
                    Application.Top.Add(new Navbar());
                    Application.Top.Add(new AppInfoBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<Volume>> cache = Result.Fail<List<Volume>>("Inital cache value");
                while (true)
                {
                    Result<List<Volume>> result = await dockerVolumeService.GetVolumeListAsync();
                    if (mainWindow.GetSelectedTab() is "Volumes" && result.IsSuccess)
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
                    // TODO
                }
            };
        }

        public static DataTable ConvertListToDataTable(List<Volume> list)
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Driver", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Size", typeof(string));
            foreach (var item in list)
            {
                table.Rows.Add(item.Name, item.Driver, item.Status, item.Size);
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

        public void ShowContextMenu(Point screenPoint, string volumeName)
        {
            contextMenu =
                new ContextMenu(screenPoint.X, screenPoint.Y,
                    new MenuBarItem(new MenuItem[]
                    {
                        new MenuItem ("Inspect", "Inspect volume", () =>
                        {
                            if (volumeName is not null)
                            {
                                Application.Top.RemoveAll();
                                var imageWindow = new ImageWindow(volumeName);
                                Application.Top.Add(imageWindow);
                                Application.Top.Add(new Navbar());
                                Application.Top.Add(new AppInfoBar());
                            }
                        }),
                        new MenuItem ("Delete", "Delete image", async () =>
                        {
                            await dockerVolumeService.DeleteVolumeAsync(volumeName);
                        }),
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
