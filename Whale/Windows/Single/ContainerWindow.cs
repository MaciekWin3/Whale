using Terminal.Gui;
using Whale.Windows.Single.ContainerTabs;

namespace Whale.Windows.Single
{
    public class ContainerWindow : Window
    {
        public string ContainerId { get; init; }
        public ContainerWindow(string containerId) : base("Container: " + containerId)
        {
            ContainerId = containerId;
            InitView();
        }

        public void InitView()
        {
            var tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(100),
                Height = Dim.Fill(),
            };

            tabView.AddTab(new TabView.Tab("Logs", new ContainerLogsWindow()), false);
            tabView.AddTab(new TabView.Tab("Inspect", new ContainerInspectWindow()), false);
            tabView.AddTab(new TabView.Tab("Terminal", new ContainerTerminalWindow()), false);
            tabView.AddTab(new TabView.Tab("Files", new ContainerFilesWindow()), false);
            tabView.AddTab(new TabView.Tab("Stats", new ContainerStatsWindow()), false);

            Add(tabView);


            //var goBack = new Button("Go back")
            //{
            //    X = 2,
            //    Y = 2,
            //};
            //goBack.Clicked += () =>
            //{
            //    Application.Top.RemoveAll();
            //    var mainWindow = MainWindow.CreateAsync();
            //    Application.Top.Add(mainWindow);
            //    Application.Top.Add(MenuBarX.CreateMenuBar());
            //    Application.Refresh();
            //};
            //Add(goBack);
        }

    }
}
