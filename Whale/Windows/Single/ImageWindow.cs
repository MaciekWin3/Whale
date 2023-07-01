using System.Globalization;
using Terminal.Gui;
using Whale.Components;

namespace Whale.Windows.Single
{
    public sealed class ImageWindow : Window
    {
        private ContextMenu contextMenu = new();
        public string ImageId { get; init; }
        public ImageWindow(string imageId) : base("Image")
        {
            ImageId = imageId;
            InitView();
        }

        public void InitView()
        {
            ConfigureContextMenu();
            var exit = new Button("Exit", is_default: true);
            var create = new Button("Create", is_default: true);
            exit.Clicked += () => Application.RequestStop();

            var label = new Label("Image ID: " + ImageId)
            {
                X = 5,
                Y = 5,
            };
            Add(label);

            var showDialog = new Button("Show dialog")
            {
                X = 6,
                Y = Pos.Bottom(label),
            };
            showDialog.Clicked += () =>
            {
                Dialog dialog = null!;
                dialog = new Dialog("Image: " + ImageId, 60, 20);
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
                var portsKeyField = new TextField("")
                {
                    X = 0,
                    Y = Pos.Bottom(portsLabel),
                    Width = Dim.Percent(50),
                };
                var portsValueField = new TextField("")
                {
                    X = Pos.Right(portsKeyField),
                    Y = Pos.Bottom(portsLabel),
                    Width = Dim.Percent(50)
                };
                dialog.Add(label, textField, portsLabel, portsKeyField, portsValueField);
                dialog.AddButton(create);
                dialog.AddButton(exit);
                Application.Run(dialog);
            };
            Add(showDialog);
        }

        public void ConfigureContextMenu()
        {
            Point mousePos = default;

            KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == (Key.m))
                {
                    ShowContextMenu(mousePos.X, mousePos.Y);
                    e.Handled = true;
                }
            };

            MouseClick += (e) =>
            {
                if (e.MouseEvent.Flags == contextMenu.MouseFlags)
                {
                    ShowContextMenu(e.MouseEvent.X, e.MouseEvent.Y);
                    e.Handled = true;
                }
            };
            Application.RootMouseEvent += Application_RootMouseEvent;

            void Application_RootMouseEvent(MouseEvent me)
            {
                mousePos = new Point(me.X, me.Y);
            }

            WantMousePositionReports = true;

            Application.Top.Closed += (_) =>
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Application.RootMouseEvent -= Application_RootMouseEvent;
            };
        }

        public void ShowContextMenu(int x, int y)
        {
            contextMenu = new ContextMenu(x, y,
                new MenuBarItem(new MenuItem[]
                {
                    new MenuItem ("_Configuration", "Show configuration", () => MessageBox.Query (50, 5, "Info", "This would open settings dialog", "Ok")),
                    new MenuBarItem ("More options", new MenuItem []
                    {
                        new MenuItem ("_Setup", "Change settings", () => MessageBox.Query (50, 5, "Info", "This would open setup dialog", "Ok")),
                        new MenuItem ("_Maintenance", "Maintenance mode", () => MessageBox.Query (50, 5, "Info", "This would open maintenance dialog", "Ok")),
                    }),
                    new MenuItem ("_Quit", "", () => Application.RequestStop ()),
                    new MenuItem ("_Go back", "", () =>
                    {
                        Application.Top.RemoveAll();
                        var mainWindow = MainWindow.CreateAsync();
                        Application.Top.Add(mainWindow);
                        Application.Top.Add(MenuBarX.CreateMenuBar());
                        Application.Refresh();
                    })
                })
            )
            { ForceMinimumPosToZero = true };

            contextMenu.Show();
        }
    }
}
