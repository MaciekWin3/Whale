using Terminal.Gui;
using Whale;
using Whale.Views;

var text = await ShellCommandRunner.RunCommandAsync("docker", "ps", "-a");

Application.Init();
var top = Application.Top;
top.Add(CreateMenuBar(text.Value.std));
top.Add(await MainWindow.CreateAsync());
top.Add(new MainWindow());
Application.Refresh();
Application.Run();
Application.Shutdown();


static MenuBar CreateMenuBar(string x)
{
    return new MenuBar(new MenuBarItem[]
    {
        new MenuBarItem(x, new MenuItem []
        {
            new MenuItem("Quit", "Quit App", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.C)
        })
    });
}
// Write me what design pattern i should use to use async method in constructor
// Write me how to use async method in constructor












