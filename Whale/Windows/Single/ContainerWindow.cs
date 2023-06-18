using Terminal.Gui;
using Whale.Components;

namespace Whale.Windows.Single
{
    public class ContainerWindow : Window
    {
        public string ContainerId { get; init; }
        public ContainerWindow(string containerId) : base("Image")
        {
            ContainerId = containerId;
            InitView();
        }

        public void InitView()
        {
            var label = new Label("Container ID: " + ContainerId)
            {
                X = 5,
                Y = 5,
            };
            Add(label);

            var goBack = new Button("Go back")
            {
                X = 6,
                Y = Pos.Bottom(label),
            };
            goBack.Clicked += () =>
            {
                Application.Top.RemoveAll();
                var mainWindow = MainWindow.CreateAsync();
                Application.Top.Add(mainWindow);
                Application.Top.Add(MenuBarX.CreateMenuBar());
                Application.Refresh();
            };
            Add(goBack);
        }

    }
}
