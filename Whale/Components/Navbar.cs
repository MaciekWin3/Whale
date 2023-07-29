using System.Diagnostics;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace Whale.Components
{
    public class Navbar : MenuBar
    {
        public Navbar()
        {
            Menus = new MenuBarItem[]
            {
                new MenuBarItem("Whale", CreateMainMenuBar()),
                new MenuBarItem("Compose", CreateComposeMenuBar()),
                new MenuBarItem("Help", CreateMenuItems()),
            };
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

        private static List<MenuItem[]> CreateComposeMenuBar()
        {
            var menuItems = new List<MenuItem[]>();
            var fileMenu = new MenuItem[]
            {
                new MenuItem("Open", "", () => OpenFileDialog()),
            };
            menuItems.Add(fileMenu);
            return menuItems;
        }

        private static List<MenuItem[]> CreateMenuItems()
        {
            var menuItems = new List<MenuItem[]>();
            var fileMenu = new MenuItem[]
            {
                new MenuItem("About", "App info", () => MessageBox.Query(
                                       "About",
                                        """
                                        Whale is a terminal 
                                        """, "Close")),
                new MenuItem("Shortcuts", "List of shortcuts", () => MessageBox.Query(
                                       "Shortcuts",
                                       """
                                        Ctrl + b - Back
                                        Ctrl + t - Open terminal dialog 
                                        """, "Close")),
                new MenuItem("Readme.md", "", () => OpenUrl("https://github.com/MaciekWin3/Whale"))
            };
            menuItems.Add(fileMenu);
            return menuItems;
        }

        static void OpenFileDialog()
        {
            var fileDialog = new OpenDialog("Open", "Open a file")
            {
                AllowsMultipleSelection = false,
            };

            Application.Run(fileDialog);


            if (!fileDialog.Canceled && !string.IsNullOrWhiteSpace(fileDialog.FilePath?.ToString()))
            {
                Open(fileDialog?.FilePath?.ToString()!);
            }
        }

        private static void Open(string filename)
        {
            throw new NotImplementedException();
        }

        static void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                    {
                        CreateNoWindow = true
                    });
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
