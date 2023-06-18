using Terminal.Gui;
using Whale.Components;

namespace Whale.Windows.Single
{
    public class ImageWindow : Window
    {
        public string ImageId { get; init; }
        public ImageWindow(string imageId) : base("Image")
        {
            ImageId = imageId;
            InitView();
        }

        public void InitView()
        {
            var label = new Label("Image ID: " + ImageId)
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
                var mainWindow = new MainWindow();
                Application.Top.Add(mainWindow);
                Application.Top.Add(MenuBarX.CreateMenuBar());
                Application.Refresh();
            };
            Add(goBack);
        }
    }
}
