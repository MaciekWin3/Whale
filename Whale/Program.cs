using Terminal.Gui;
using Whale.Components;
using Whale.Services;
using Whale.Windows;

var shellCommandRunner = new ShellCommandRunner();
var dockerService = new DockerUtilityService(shellCommandRunner);

Application.Init();
InitApp(Application.Top);

static void InitApp(Toplevel top)
{
    top.Add(new Navbar());
    top.Add(new AppInfoBar());
    //top.Add(await MainWindow.CreateAsync());
    top.Add(MainWindow.CreateAsync());
    Application.Run();
    Application.Shutdown();
}

