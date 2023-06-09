using Terminal.Gui;
using Whale.Services;

namespace Whale.Windows
{
    public class ContainerWindow : Window
    {
        private ShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public ContainerWindow()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
        }

        public void InitView()
        {
            var imagesFrame = new FrameView("Containers");

            var text = new Label()
            {
                Text = "Hi, this are your containers. Use them wisly",
            };
            imagesFrame.Add(text);
            Add(imagesFrame);
            Add(text);
        }
    }
}
