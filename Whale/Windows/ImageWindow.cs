using NStack;
using Terminal.Gui;

namespace Whale.Windows
{
    public class ImageWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        public ImageWindow(Action<int, int> showContextMenu)
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
            var items = new List<ustring>()
            {
                "aaaa",
                "bbb",
                "Item1",
                "Item2",
                "Item3",
                "xxx"
            };

            var listview = new ListView(items)
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(2),
                Width = Dim.Percent(20),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

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
