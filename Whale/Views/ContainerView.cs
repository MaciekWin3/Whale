using Terminal.Gui;

namespace Whale.Views
{
    public class ContainerView : View
    {
        public ContainerView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.Rounded,
                Effect3D = false,
                Title = "Whale Dashboard"
            };
            InitView();
        }

        public void InitView()
        {
            var imagesFrame = new FrameView("Containers");

            var text = new Label()
            {
                Text = "Hi, this are your containers. Use them wisly",
            };
            imagesFrame.Add(text);
            Add(imagesFrame);
            Add(text);
        }
    }
}
