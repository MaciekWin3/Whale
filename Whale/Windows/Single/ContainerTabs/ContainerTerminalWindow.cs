using Terminal.Gui;

namespace Whale.Windows.Single.ContainerTabs
{
    public class ContainerTerminalWindow : Window
    {
        public ContainerTerminalWindow() : base()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
        }
        public void InitView()
        {
            var textField = new TextView()

            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
                ReadOnly = true
            };
            Add(textField);

        }
    }
}
