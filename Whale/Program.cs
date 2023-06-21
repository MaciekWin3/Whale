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
InitApp(Application.Top);

static void InitApp(Toplevel top)
{
    top.Add(MenuBarX.CreateMenuBar());
    //top.Add(await MainWindow.CreateAsync());
    top.Add(MainWindow.CreateAsync());
    Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Magenta, Color.BrightBlue);
    Application.Run();
    Application.Shutdown();
}
