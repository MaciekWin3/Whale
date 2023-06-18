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
        public ListView ListView { get; set; } = new();
        public List<string> ContainerList { get; set; } = new();

        private readonly Dictionary<string, Delegate> events = new();
        public ContainerListWindow(Dictionary<string, Delegate> events)
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
        }

        public void InitView()
        {
            ContainerList = new List<string>() { };

            Result<List<ContainerDTO>> containers;

            ListView = new ListView(ContainerList)
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
                        ContainerList = containers.Value?.Select(x => x.Id.ToString()).ToList();
                        ListView.SetSource(ContainerList);
                    }
                }
            });

            ListView.KeyDown += (e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    events["ShowContextMenu"].DynamicInvoke(1, 1);
                }
            };

            ListView.OpenSelectedItem += (e) =>
            {
                var name = e.Value.ToString();
                if (name is not null)
                {
                    Application.Top.RemoveAll();
                    var containerWindow = new ContainerWindow(name);
                    Application.Top.Add(containerWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };
            Add(ListView);
        }

        public string GetCurrnetContainerName()
        {
            var currentContainer = ListView.SelectedItem;
            return ContainerList[currentContainer];
        }
    }
}
