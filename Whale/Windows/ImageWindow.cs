using Terminal.Gui;
using Whale.Models;
using Whale.Objects.Image;
using Whale.Services;
using Whale.Utils;

namespace Whale.Windows
{
    public class ImageWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public ImageWindow(Action<int, int> showContextMenu) : base()
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

            Result<List<ImageDTO>> images;

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
                Result<List<ImageDTO>> cache = Result.Fail<List<ImageDTO>>("Initial cache value");
                while (true)
                {
                    Result<List<ImageDTO>> result = await dockerService.GetImageListAsync();
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
                        images = await dockerService.GetImageListAsync();
                        var cont = images.Value?.Select(x => x.Name.ToString()).ToList();
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
                var x = await dockerService.GetDockerObjectInfoAsync<Image>(name);
                MessageBox.Query(50, 7, name,
                    $"""
                     Name: {x?.Value?.Id}
                     Platform: {x?.Value?.Os}
                     """,
                    "Ok");
            };
            Add(listview);
        }
    }
}
