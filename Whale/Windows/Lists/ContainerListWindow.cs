using System.Data;
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
        private MainWindow mainWindow;
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
            ContainerList = new List<string>() { };

            Result<List<ContainerDTO>> containers;

            // Table Editor for the container list
            var tableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
            };
            // When i select row any cell show me the container id
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
                Result<List<ContainerDTO>> cache = Result.Fail<List<ContainerDTO>>("Initial cache value");
                while (true)
                {
                    Result<List<ContainerDTO>> result = await dockerService.GetContainerListAsync();
                    if (mainWindow.GetSelectedTab() is "Containers")
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
                    events["ShowContextMenu"].DynamicInvoke(1, 1);
                }
            };

            //Add(ListView);
            Add(tableView);
        }

        public static DataTable ConvertListToDataTable(List<ContainerDTO> list)
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Image", typeof(string));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("Created", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Ports", typeof(string));
            table.Columns.Add("Names", typeof(string));

            foreach (var item in list)
            {
                table.Rows.Add(item.Id, item.Image, item.Command, item.CreatedDate, item.Status, item.Ports, item.Names);
            }
            return table;
        }
    }
}
