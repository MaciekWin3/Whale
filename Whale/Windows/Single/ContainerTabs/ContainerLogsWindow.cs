using Terminal.Gui;
using Whale.Components;

namespace Whale.Windows.Single.ContainerTabs
{
    public class ContainerLogsWindow : Window
    {
        public ContainerLogsWindow() : base()
        {
            InitView();
        }

        public void InitView()
        {
            var label = new Label("Test Window")
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
