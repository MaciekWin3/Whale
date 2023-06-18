using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public class ImageListWindow : Window, IDisposable
    {
        readonly Action<int, int> showContextMenu;
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public ImageListWindow(Action<int, int> showContextMenu) : base()
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
                Width = Dim.Fill(),
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

            listview.KeyDown += (e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    showContextMenu.Invoke(1, 1);
                }
            };
            listview.OpenSelectedItem += (e) =>
            {
                var name = e.Value.ToString();
                if (name is not null)
                {
                    Application.Top.RemoveAll();
                    var imageWindow = new ImageWindow(name);
                    Application.Top.Add(imageWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };
            Add(listview);
        }
    }
}
