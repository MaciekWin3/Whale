using Terminal.Gui;
using Whale.Models;
using Whale.Objects.Container;
using Whale.Services;
using Whale.Utils;

namespace Whale.Windows
{
    public class ContainerWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public ContainerWindow(Action<int, int> showContextMenu)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            this.showContextMenu = showContextMenu;
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
        }

        public void InitView()
        {
            var items = new List<string>() { };

            Result<List<ContainerDTO>> containers;

            var listview = new ListView(items)
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(2),
                Width = Dim.Percent(40),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                Result<List<ContainerDTO>> cache = Result.Fail<List<ContainerDTO>>("Initial cache value");
                while (true)
                {
                    Result<List<ContainerDTO>> result = await dockerService.GetContainerListAsync();
                    if (!result.IsSuccess)
                    {
                        continue;
                    }
                    if (cache.IsSuccess && cache.Value.SequenceEqual(result.Value))
                    {
                        cache = result;
                    }
                    else
                    {
                        cache = result;
                        containers = await dockerService.GetContainerListAsync();
                        var cont = containers.Value?.Select(x => x.Id.ToString()).ToList();
                        listview.RemoveAll();
                        listview.SetSource(cont);
                    }
                }
            });

            listview.KeyDown += (KeyEventEventArgs e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    showContextMenu.Invoke(1, 1);
                }
            };
            listview.OpenSelectedItem += async (ListViewItemEventArgs e) =>
            {
                var name = e.Value.ToString();
                var x = await dockerService.GetDockerObjectInfoAsync<Container>(name);
                MessageBox.Query(50, 7, name,
                    $"""
                     Name: {x?.Value?.Name}
                     Platform: {x?.Value?.Platform}
                     Driver: {x?.Value?.Driver}
                     """,
                    "Ok");
            };
            Add(listview);
        }
    }
}
