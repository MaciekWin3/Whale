using System.Data;
using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Windows.Images
{
    public sealed class ImageListWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerImageService dockerImageService;
        private readonly MainWindow mainWindow;
        public ImageListWindow(MainWindow mainWindow)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerImageService = new DockerImageService(shellCommandRunner);
            X = 0;
            Y = 0;
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
                    var imageWindow = new ImageWindow(name);
                    Application.Top.Add(imageWindow);
                    Application.Top.Add(new Navbar());
                    Application.Top.Add(new AppInfoBar(""));
                    Application.Refresh();
                }
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<Image>> cache = Result.Fail<List<Image>>("Initial cache value");
                while (true)
                {
                    Result<List<Image>> result = await dockerImageService.GetImageListAsync();
                    if (mainWindow.GetSelectedTab() is "Images" && result.IsSuccess)
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

        public static DataTable ConvertListToDataTable(List<Image> list)
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Tag", typeof(string));
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Created", typeof(string));
            table.Columns.Add("Size", typeof(string));

            foreach (var item in list)
            {
                table.Rows.Add(item.Repository, item.Tag, item.ID, item.CreatedSince, item.Size);
            }
            return table;
        }
    }
}
