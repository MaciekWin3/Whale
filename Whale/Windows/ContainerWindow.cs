using Terminal.Gui;

namespace Whale.Windows
{
    public class ContainerWindow : Window
    {
        public ContainerWindow()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
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
