using Terminal.Gui;

namespace Whale.Views
{
    public class ImageView : View
    {
        public ImageView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            InitView();
        }

        public void InitView()
        {
            var imagesFrame = new FrameView("Images");

            var text = new Label()
            {
                Text = "Hi, this are your images. Use them wisly",
            };
            imagesFrame.Add(text);
            Add(imagesFrame);
            Add(text);
        }
    }
}
