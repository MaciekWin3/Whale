using Terminal.Gui;

namespace Whale.Components
{
    public class MenuBarX
    {
        public static MenuBar CreateMenuBar()
        {
            var menuBar = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("Whale", CreateSomething())
            });

            return menuBar;
        }

        private static List<MenuItem[]> CreateSomething()
        {
            var menuItems = new List<MenuItem[]>();
            var fileMenu = new MenuItem[]
            {
                new MenuItem("Open", "", () => MessageBox.Query("Hello", "This is something")),
            };
            menuItems.Add(fileMenu);
            menuItems.Add(CreateMenuItems());
            return menuItems;
        }

        private static MenuItem[] CreateMenuItems()
        {
            List<MenuItem> menuItems = new();
            var quit = new MenuItem("Quit", "", () => Application.RequestStop());
            menuItems.Add(quit);
            return menuItems.ToArray();
        }
    }
}
