using System.Diagnostics;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace Whale.Components
{
    public class MenuBarX
    {
        public static MenuBar CreateMenuBar()
        {
            var menuBar = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("Whale", CreateMainMenuBar()),
                new MenuBarItem("Help", CreateMenuItems()),
            });

            return menuBar;
        }

        private static List<MenuItem[]> CreateMainMenuBar()
        {
            var menuItems = new List<MenuItem[]>();
            var fileMenu = new MenuItem[]
            {
                new MenuItem("Open", "", () => MessageBox.Query("Hello", "This is something")),
                new MenuItem("Quit", "", () => Application.RequestStop()),
            };
            menuItems.Add(fileMenu);
            return menuItems;
        }

        private static List<MenuItem[]> CreateMenuItems()
        {
            var menuItems = new List<MenuItem[]>();
            var fileMenu = new MenuItem[]
            {
                new MenuItem("About", "", () => MessageBox.Query("Hello", "This is something")),
                new MenuItem("Readme.md", "", () => OpenUrl("https://github.com/MaciekWin3/Whale"))
            };
            menuItems.Add(fileMenu);
            return menuItems;

        }

        static void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = url,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
