using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Containers.Tabs
{
    public sealed class ContainerInspectWindow : Toplevel
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerUtilityService dockerUtilityService;

        public string ContainerId { get; set; }
        public ContainerInspectWindow(string containerId)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerUtilityService = new DockerUtilityService(shellCommandRunner);

            ContainerId = containerId;
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            ColorScheme = Colors.Base;
        }
        public void InitView()
        {
            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1
            };

            Add(textView);

            //Application.MainLoop.Invoke(async () =>
            //{
            //    var result = await shellCommandRunner.RunCommandAsync("docker container inspect " + ContainerId);
            //    textView.Text = result.Value.std;
            //});
        }
    }
}
