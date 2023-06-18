using Terminal.Gui;

namespace Whale.Windows.Single
{
    public class ContainerWindow : Window
    {
        public string ContainerId { get; init; }
        public ContainerWindow(string imageId) : base("Image")
        {
            ContainerId = imageId;
            InitView();
        }

        public void InitView()
        {
            var label = new Label("Container ID: " + ContainerId)
            {
                X = 5,
                Y = 5,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            Add(label);
        }

    }
}
