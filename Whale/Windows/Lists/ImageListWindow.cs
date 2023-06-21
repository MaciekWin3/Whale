using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public class ImageListWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        private MainWindow mainWindow;
        public ImageListWindow(Action<int, int> showContextMenu, MainWindow mainWindow) : base()
        {
            X = 0;
            Y = 0;
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
                    var containerWindow = new ContainerWindow(name);
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<ImageDTO>> cache = Result.Fail<List<ImageDTO>>("Initial cache value");
                while (true)
                {
                    Result<List<ImageDTO>> result = await dockerService.GetImageListAsync();
                    if (mainWindow.GetSelectedTab() is "Images")
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

        public static DataTable ConvertListToDataTable(List<ImageDTO> list)
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Tag", typeof(string));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("CreatedData", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Ports", typeof(string));
            table.Columns.Add("Names", typeof(string));

            foreach (var item in list)
            {
                table.Rows.Add(item.Name, item.Tag, item.Command, item.CreatedDate, item.Status, item.Ports, item.Names);
            }
            return table;
        }
    }
}
