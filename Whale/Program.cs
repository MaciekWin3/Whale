using Terminal.Gui;
using Whale.Components;
using Whale.Services;
using Whale.Windows;

var cellHighlight = new ColorScheme()
{
    Normal = Terminal.Gui.Attribute.Make(Color.BrightCyan, Color.DarkGray),
    HotNormal = Terminal.Gui.Attribute.Make(Color.Green, Color.Blue),
    HotFocus = Terminal.Gui.Attribute.Make(Color.BrightYellow, Color.White),
    Focus = Terminal.Gui.Attribute.Make(Color.Cyan, Color.Magenta),
};

var shellCommandRunner = new ShellCommandRunner();
var dockerService = new DockerUtilityService(shellCommandRunner);

string? version = "Unkown";
try
{
    var dockerInfo = await dockerService.GetDockerVersionObjectAsync();
    version = dockerInfo?.Value?.Client?.Version;
}
catch
{
}

Application.Init();
InitApp(Application.Top);

void InitApp(Toplevel top)
{
    top.Add(MenuBarX.CreateMenuBar());
    top.Add(AppInfoBar.Create(version ?? "Unknown"));
    //top.Add(await MainWindow.CreateAsync());
    top.Add(MainWindow.CreateAsync());
    //Colors.Base.Normal = Application.Driver.MakeAttribute(Color.Green, Color.Black);
    Application.Run();
    Application.Shutdown();
}

