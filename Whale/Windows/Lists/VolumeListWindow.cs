using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public class VolumeListWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly ShellCommandRunner shellCommandRunner = new();
        private readonly MainWindow mainWindow;
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public VolumeListWindow(Action<int, int> showContextMenu, MainWindow mainWindow) : base()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
            };
            this.showContextMenu = showContextMenu;
            InitView();
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
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<VolumeDTO>> cache = Result.Fail<List<VolumeDTO>>("Inital cache value");
                while (true)
                {
                    Result<List<VolumeDTO>> result = await dockerService.GetVolumeListAsync();
                    if (mainWindow.GetSelectedTab() is "Volumes")
                    {
                        if (cache.IsSuccess && cache.Value.SequenceEqual(result.Value))
                        {
                            continue;
                        }
                        else
                        {
                            tableView.Table = ConvertListToDataTable(result.Value);
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
                    showContextMenu.Invoke(1, 1);
                }
            };
        }

        // More info?
        public DataTable ConvertListToDataTable(List<VolumeDTO> list)
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Driver", typeof(string));
            foreach (var item in list)
            {
                table.Rows.Add(item.Name, item.Driver);
            }
            return table;
        }
    }
}
