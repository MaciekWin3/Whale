using Terminal.Gui;

namespace Whale.Windows.Single
{
    public class VolumeWindow : Window
    {
        public string VolumeId { get; init; }
        public VolumeWindow(string imageId) : base("Image")
        {
            VolumeId = imageId;
            InitView();
        }
        public void InitView()
        {
            var label = new Label("Volume ID: " + VolumeId)
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
