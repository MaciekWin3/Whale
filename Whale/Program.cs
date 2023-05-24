//ShellCommandRunner.RunCommand("docker ps -a");
using Terminal.Gui;
using Whale.Windows;

Application.Init();
var top = Application.Top;
top.Add(CreateMenuBar());
top.Add(new MainWindow());
Application.Run();
Application.Shutdown();

static MenuBar CreateMenuBar()
{
    return new MenuBar(new MenuBarItem[]
    {
        new MenuBarItem("App", new MenuItem []
        {
            new MenuItem("Quit", "Quit App", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C)
        })
    });
}
