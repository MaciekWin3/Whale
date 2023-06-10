using Terminal.Gui;
using Whale.Models;
using Whale.Objects.Container;
using Whale.Objects.Volume;
using Whale.Services;
using Whale.Utils;

namespace Whale.Windows
{
    public class VolumeWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly ShellCommandRunner shellCommandRunner = new();
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public VolumeWindow(Action<int, int> showContextMenu) : base()
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

            Result<List<VolumeDTO>> images;

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
                        images = await dockerService.GetVolumeListAsync();
                        var cont = images.Value?.Select(x => x.Name.ToString()).ToList();
                        listview.RemoveAll();
                        listview.SetSource(cont);
                    }
                    else
                    {
                        cache = result.Value.std;
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
                var x = await dockerService.GetDockerObjectInfoAsync<Volume>(name);
                var z = await dockerService.GetDockerObjectInfoAsync<Container>("a6e319bfe8b7");
                MessageBox.Query(50, 7, name,
                    $"""
                     Name: {x?.Value?.Name}
                     Mountpoint: {x?.Value?.Mountpoint}
                     Driver: {x?.Value?.Driver}
                     """,
                    "Ok");
            };
            Add(listview);
        }
    }
}
