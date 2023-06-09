using Terminal.Gui;
using Whale.Models;
using Whale.Services;
using Whale.Utils;

namespace Whale.Windows
{
    public class ImageWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private ShellCommandRunner shellCommandRunner = new();
        private readonly IDockerService dockerService = new DockerService(new ShellCommandRunner());
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

        public async void InitView()
        {
            var items = new List<string>()
            {
            };

            Result<List<Container>> images;

            var listview = new ListView(items)
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(2),
                Width = Dim.Percent(40),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

            Application.MainLoop.Invoke(async () =>
            {
                images = await dockerService.GetContainerListAsync();
                var cont = images.Value?.Select(x => x.Id.ToString()).ToList();
                listview.RemoveAll();
                listview.SetSource(cont);
                Application.Refresh();
            });

            listview.KeyDown += (KeyEventEventArgs e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    // i want to context menu to pop up
                    showContextMenu.Invoke(1, 1);
                    //var selected = listview.SelectedItem;
                    //var selectedText = items[selected];
                    //var dialog = new Dialog(selectedText.ToString(), 60, 20, new Button("Ok", is_default: true));
                    //Application.Run(dialog);
                }
            };
            //listview.SelectedItemChanged += (ListViewItemEventArgs e) => lbListView.Text = items[listview.SelectedItem];
            listview.OpenSelectedItem += (ListViewItemEventArgs e) =>
            {
                if (e.Value.ToString() == "bbb")
                {
                    MessageBox.Query(50, 7, "Error", "Error message", "Ok");
                }
            };
            Add(listview);
        }
    }
}
