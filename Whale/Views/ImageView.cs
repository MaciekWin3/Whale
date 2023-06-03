using NStack;
using Terminal.Gui;

namespace Whale.Views
{
    public class ImageView : View
    {
        public ImageView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            InitView();
        }

        public void InitView()
        {
            var imagesFrame = new FrameView("Images");
            Add(imagesFrame);

            var items = new List<ustring>();
            foreach (var dir in new[] { "/etc", @$"{Environment.GetEnvironmentVariable("SystemRoot")}\System32" })
            {
                if (Directory.Exists(dir))
                {
                    items = Directory.GetFiles(dir).Union(Directory.GetDirectories(dir))
                        .Select(Path.GetFileName)
                        .Where(x => char.IsLetterOrDigit(x[0]))
                        .OrderBy(x => x).Select(x => ustring.Make(x)).ToList();
                }
            }

            // ListView
            var lbListView = new Label("Listview")
            {
                ColorScheme = Colors.Base,
                X = 0,
                Width = Dim.Fill(),
            };

            var listview = new ListView(items)
            {
                X = 0,
                Y = Pos.Bottom(lbListView) + 1,
                Height = Dim.Fill(),
                Width = Dim.Fill()
            };
            listview.SelectedItemChanged += (ListViewItemEventArgs e) => lbListView.Text = items[listview.SelectedItem];
            Add(lbListView, listview);
        }
    }
}
