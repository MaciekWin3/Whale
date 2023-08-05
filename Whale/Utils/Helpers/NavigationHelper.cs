using Terminal.Gui;
using Whale.Components;
using Whale.Windows;

namespace Whale.Utils.Helpers
{
    public static class NavigationHelper
    {
        public static void ReturnToMainWindow(string selectedTab)
        {
            /*
                Containers = 0,
                Images = 1,
                Volumes = 2,
            */

            Application.Top.RemoveAll();
            var mainWindow = MainWindow.CreateAsync();

            int tabIndex = selectedTab switch
            {
                "Containers" => 0,
                "Images" => 1,
                "Volumes" => 2,
                _ => throw new InvalidOperationException("No tab found!"),
            };

            Application.Top.Add(mainWindow);
            //mainWindow.tabView.TabScrollOffset = 0;
            mainWindow.TabView.SwitchTabBy(tabIndex);
            Application.Top.Add(new Navbar());
            Application.Top.Add(new AppInfoBar());
        }
    }
}
