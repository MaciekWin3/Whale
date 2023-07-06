using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public sealed class ContainerListWindow : Toplevel
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

            alternatingColorScheme = new ColorScheme()
            {
                Disabled = ColorScheme.Disabled,
                HotFocus = ColorScheme.HotFocus,
                Focus = ColorScheme.Focus,
                Normal = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Blue)
            };

            tableView.Style.SmoothHorizontalScrolling = true;
            tableView.Style.ExpandLastColumn = true;
            tableView.Style.RowColorGetter = (a) =>
            {
                if (a.Table.Rows[a.RowIndex][3].ToString().Contains("Up"))
                {
                    return alternatingColorScheme;
                }
                return null;
            };

            tableView.CellActivated += (e) =>
            {
                int row = e.Row;
                var id = (string)e.Table.Rows[row][0];
                if (id is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(id);
                    mainWindow.Dispose();
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
