using Terminal.Gui;

namespace Whale.Windows.Single.ContainerTabs
{
    public class ContainerLogsWindow : Window
    {
        public ContainerLogsWindow() : base()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
        }

        public void InitView()
        {
            var textField = new TextView
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
                DesiredCursorVisibility = CursorVisibility.Vertical,
            };
            Add(textField);
        }
    }
}
