using Terminal.Gui;

namespace Whale.Windows.Single.ContainerTabs
{
    public sealed class ContainerFilesWindow : Toplevel
    {
        public ContainerFilesWindow()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
        }

        public void InitView()
        {
            // File sysytem exploer
            var treeView = new TreeView
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
            };

            Add(treeView);
        }
    }
}
