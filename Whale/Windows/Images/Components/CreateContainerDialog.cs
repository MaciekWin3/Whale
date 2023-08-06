using Terminal.Gui;
using Whale.Services;
using Whale.Services.Interfaces;

namespace Whale.Windows.Images.Components
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

            // Name
            var nameLabel = new Label("Container Name:")
            {
                X = 0,
                Y = 1,
            };
            var nameField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(nameLabel),
                Width = Dim.Fill()
            };

            // Ports
            var portsLabel = new Label("Port mappings (ex. 8080:80, 8081:433):")
            {
                X = 0,
                Y = Pos.Bottom(nameField)
            };
            var portsField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(portsLabel),
                Width = Dim.Fill(),
            };

            // Environment variables
            var envLabel = new Label("Environment variables (ex. key=value, key2=value2:")
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

            // Volumes
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

            // Additional options
            var additionalOptionsLabel = new Label("Additional flags:")
            {
                X = 0,
                Y = Pos.Bottom(volumesField)
            };
            var additionalOptionsField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(additionalOptionsLabel),
                Width = Dim.Fill(),
            };

            // Entry point
            var entryPointLabel = new Label("Entry point:")
            {
                X = 0,
                Y = Pos.Bottom(additionalOptionsField)
            };
            var entryPointField = new TextField("")
            {
                X = 0,
                Y = Pos.Bottom(entryPointLabel),
                Width = Dim.Fill(),
            };

            var create = new Button("Create");

            create.Clicked += async () =>
            {
                var containerName = nameField.Text.ToString();
                var ports = portsField.Text.ToString();
                var env = envField.Text.ToString();
                var volumes = volumesField.Text.ToString();
                var additionalOptions = additionalOptionsField.Text.ToString();

                var portMappings = SplitAndFormatOptions(ports!, "-p");
                var environmentVariables = SplitAndFormatOptions(env!, "-e");
                var volumeMappings = SplitAndFormatOptions(volumes!, "-v");

                var parameters = $"--name {containerName} {portMappings} {environmentVariables} {volumeMappings} {additionalOptions} {ImageId} {entryPointField.Text}";

                var result = await dockerContainerService
                    .CreateContainerAsync(parameters);

                if (result.IsSuccess)
                {
                    MessageBox.Query(50, 7, "Success", "Container created successfully", "Ok");
                }
                else
                {
                    MessageBox.ErrorQuery(50, 7, "Error", result.Error, "Ok");
                }
            };

            Add(nameLabel, nameField,
                portsLabel, portsField,
                envLabel, envField,
                volumesLabel, volumesField,
                additionalOptionsLabel, additionalOptionsField,
                entryPointLabel, entryPointField);
            AddButton(create);
            AddButton(exit);
        }

        private string SplitAndFormatOptions(string input, string optionFlag)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var options = input
                .Split(',')
                .Select(entry => $"{optionFlag} {entry.Trim()}");

            return string.Join(" ", options);
        }

        public void ShowDialog()
        {
            Application.Run(this);
        }
    }
}

