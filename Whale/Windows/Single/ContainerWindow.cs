using Terminal.Gui;
using Whale.Windows.Single.ContainerTabs;

namespace Whale.Windows.Single
{
    public sealed class ContainerWindow : Window
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

            tabView.AddTab(new TabView.Tab("Logs", new ContainerLogsWindow(ContainerId)), false);
            tabView.AddTab(new TabView.Tab("Inspect", new ContainerInspectWindow()), false);
            tabView.AddTab(new TabView.Tab("Terminal", new ContainerTerminalWindow(ContainerId)), false);
            tabView.AddTab(new TabView.Tab("Files", new ContainerFilesWindow()), false);
            tabView.AddTab(new TabView.Tab("Stats", new ContainerStatsWindow(ContainerId)), false);

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == (Key.Tab))
                {
                    var tabs = tabView.Tabs.Count;
                    if (tabView.SelectedTab == tabView.Tabs.ToArray()[tabs - 1])
                    {
                        tabView.SelectedTab = tabView.Tabs.ToArray()[0];
                    }
                    else
                    {
                        tabView.SwitchTabBy(1);
                    }
                    e.Handled = true;
                }
            };

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
