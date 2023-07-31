using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace Whale.Windows.Containers.Components
{
    class DiscoBarSeries : BarSeries
    {
        private Terminal.Gui.Attribute green;
        private Terminal.Gui.Attribute brightgreen;
        private Terminal.Gui.Attribute brightyellow;
        private Terminal.Gui.Attribute red;
        private Terminal.Gui.Attribute brightred;

        public DiscoBarSeries()
        {

            green = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Black);
            brightgreen = Application.Driver.MakeAttribute(Color.Green, Color.Black);
            brightyellow = Application.Driver.MakeAttribute(Color.BrightYellow, Color.Black);
            red = Application.Driver.MakeAttribute(Color.Red, Color.Black);
            brightred = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);
        }
        protected override void DrawBarLine(GraphView graph, Point start, Point end, Bar beingDrawn)
        {
            var driver = Application.Driver;

            int x = start.X;
            for (int y = end.Y; y <= start.Y; y++)
            {
                var height = graph.ScreenToGraphSpace(x, y).Y;

                driver.SetAttribute(height switch
                {
                    var h when h >= 85 => red,
                    var h when h >= 66 => brightred,
                    var h when h >= 45 => brightyellow,
                    var h when h >= 25 => brightgreen,
                    _ => green
                });

                graph.AddRune(x, y, beingDrawn.Fill.Rune);
            }
        }
    }
}
