using Terminal.Gui;
using Whale.Views;

Application.Init();
var top = Application.Top;
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
