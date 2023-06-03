using Terminal.Gui;
using Whale.Views;

var cellHighlight = new ColorScheme()
{
    Normal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.DarkGray),
    HotNormal = Terminal.Gui.Attribute.Make(Color.Green, Color.Blue),
    HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.White),
    Focus = Terminal.Gui.Attribute.Make(Color.Cyan, Color.Magenta),
};

Application.Init();
var top = Application.Top;
Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Gray, Color.Blue);
top.Add(CreateMenuBar());
//top.Add(await MainWindow.CreateAsync());
top.Add(MainWindow.CreateAsync());
Application.Refresh();
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
            new MenuItem("Quit", "Quit App", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C)
        })
    });
}
