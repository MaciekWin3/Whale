using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Components
{
    class CreateContainerDialog : Dialog
    {
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerContainerService dockerContainerService;
        public string ImageId { get; init; }
        public CreateContainerDialog(string imageId)
        {
            shellCommandRunner = new ShellCommandRunner();
            dockerContainerService = new DockerContainerService(shellCommandRunner);
            X = Pos.Center();
            Y = Pos.Center();
            Width = Dim.Percent(70);
            Height = Dim.Percent(70);
            Border = new Border
            {
                BorderStyle = BorderStyle.Rounded,
                Effect3D = false,
                Title = "Terminal",
                Padding = new Thickness(1, 0, 1, 0),
            };
            InitView();
            ImageId = imageId;
        }

        public void InitView()
        {
            var exit = new Button("Exit");
            exit.Clicked += () => Application.RequestStop();

            var label = new Label("Container Name:")
            {
                X = 0,
                Y = 1,
            };
            var textField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(label),
                Width = Dim.Fill()
            };
            var portsLabel = new Label("Ports:")
            {
                X = 0,
                Y = Pos.Bottom(textField)
            };
            var portsField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(portsLabel),
                Width = Dim.Fill(),
            };
            var envLabel = new Label("Environment variables:")
            {
                X = 0,
                Y = Pos.Bottom(portsField)
            };
            var envField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(envLabel),
                Width = Dim.Fill(),
            };
            var volumesLabel = new Label("Volumes:")
            {
                X = 0,
                Y = Pos.Bottom(envField)
            };
            var volumesField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(volumesLabel),
                Width = Dim.Fill(),
            };

            var create = new Button("Create");

            create.Clicked += async () =>
            {
                var containerName = textField.Text.ToString();
                var ports = portsField.Text.ToString();
                var env = envField.Text.ToString();
                var volumes = volumesField.Text.ToString();
                await dockerContainerService.CreateContainerAsync(new List<string> { "--name", "hello" });
            };

            Add(label, textField, portsLabel, portsLabel, portsField);
            AddButton(create);
            AddButton(exit);
        }

        public void ShowDialog()
        {
            Application.Run(this);
        }
    }
}

