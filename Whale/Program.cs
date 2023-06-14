using NStack;
using Terminal.Gui;
using Whale.Components;
using Whale.Windows;

var cellHighlight = new ColorScheme()
{
    Normal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.DarkGray),
    HotNormal = Terminal.Gui.Attribute.Make(Color.Green, Color.Blue),
    HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.White),
    Focus = Terminal.Gui.Attribute.Make(Color.Cyan, Color.Magenta),
};

Application.Init();
var top = Application.Top;
//Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Gray, Color.Blue);
Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Magenta, Color.BrightBlue);
//top.Add(CreateMenuBar());
top.Add(MenuBarX.CreateMenuBar());
//top.Add(await MainWindow.CreateAsync());
top.Add(MainWindow.CreateAsync());
//TestView();
Application.Run();
Application.Shutdown();

static MenuBar CreateMenuBar()
{
    return new MenuBar(new MenuBarItem[]
    {
        new MenuBarItem("Quit", new MenuItem []
        {
            new MenuItem("Open", "Open file", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C),
            new MenuItem("Update", "Update App", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C),
            new MenuItem("Quit", "Quit App", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C),

        })
    });
}

void TestView()
{
    var items = new List<ustring>()
    {
    "Item1",
    "Item2"
    };
    // ListView
    var lbListView = new Label("Listview")
    {
        ColorScheme = Colors.TopLevel,
        X = 0,
        Y = 1,
        Width = Dim.Percent(20)
    };

    var listview = new ListView(items)
    {
        X = 0,
        Y = Pos.Bottom(lbListView) + 1,
        Height = Dim.Fill(2),
        Width = Dim.Percent(20)
    };
    listview.SelectedItemChanged += (ListViewItemEventArgs e) => lbListView.Text = items[listview.SelectedItem];
    top.Add(lbListView, listview);

    var _scrollBar = new ScrollBarView(listview, true);

    _scrollBar.ChangedPosition += () =>
    {
        listview.TopItem = _scrollBar.Position;
        if (listview.TopItem != _scrollBar.Position)
        {
            _scrollBar.Position = listview.TopItem;
        }
        listview.SetNeedsDisplay();
    };

    _scrollBar.OtherScrollBarView.ChangedPosition += () =>
    {
        listview.LeftItem = _scrollBar.OtherScrollBarView.Position;
        if (listview.LeftItem != _scrollBar.OtherScrollBarView.Position)
        {
            _scrollBar.OtherScrollBarView.Position = listview.LeftItem;
        }
        listview.SetNeedsDisplay();
    };

    listview.DrawContent += (e) =>
    {
        _scrollBar.Size = listview.Source.Count - 1;
        _scrollBar.Position = listview.TopItem;
        _scrollBar.OtherScrollBarView.Size = listview.Maxlength - 1;
        _scrollBar.OtherScrollBarView.Position = listview.LeftItem;
        _scrollBar.Refresh();
    };
}
