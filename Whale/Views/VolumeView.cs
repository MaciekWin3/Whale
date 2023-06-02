using Terminal.Gui;

namespace Whale.Views
{
    public class VolumeView : View
    {
        public VolumeView()
        {
            Height = Dim.Fill();
            Width = Dim.Fill();
            InitView();
        }

        public void InitView()
        {
            var volumesFrame = new FrameView("Volumes")
            {
                Border = new Border
                {
                    Effect3D = true
                }
            };

            var text = new Label()
            {
                Text = "Hi, this are your volumes. Use them wisly",
            };
            volumesFrame.Add(text);
            Add(volumesFrame);
            Add(text);
        }
    }
}
