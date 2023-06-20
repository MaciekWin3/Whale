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
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public VolumeListWindow(Action<int, int> showContextMenu) : base()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
            };
            this.showContextMenu = showContextMenu;
            InitView();
        }

        public void InitView()
        {
            var items = new List<string>() { };

            Result<List<VolumeDTO>> volumes;

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
                    var containerWindow = new ContainerWindow(name);
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                string cache = string.Empty;
                while (true)
                {
                    Result<(string std, string error)> result
                        = await shellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                    if (!result.IsSuccess)
                    {
                        continue;
                    }
                    if (cache != result.Value.std)
                    {
                        cache = result.Value.std;
                        volumes = await dockerService.GetVolumeListAsync();
                    }
                    else
                    {
                        cache = result.Value.std;
                        volumes = await dockerService.GetVolumeListAsync();
                        tableView.Table = ConvertListToDataTable(volumes.Value);
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
        public static DataTable ConvertListToDataTable(List<VolumeDTO> list)
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
