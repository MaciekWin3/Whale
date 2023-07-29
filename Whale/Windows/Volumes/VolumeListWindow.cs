using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Windows.Volumes
{
    public sealed class VolumeListWindow : Toplevel
    {
        private readonly MainWindow mainWindow;
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerVolumeService dockerVolumeService;
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
                    Application.Top.Add(new Navbar());
                    Application.Top.Add(new AppInfoBar(""));
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

        // More info?
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
    }
}
