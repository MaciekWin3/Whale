using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;

namespace Whale.Windows.Single.ContainerTabs
{
    public class ContainerStatsWindow : Window
    {
        GraphView graphView = null!;
        public ContainerStatsWindow() : base()
        {
            Border = new Border
            {
                BorderStyle = BorderStyle.None,
            };
            InitView();
        }
        public void InitView()
        {
            graphView = new GraphView()
            {
                X = 1,
                Y = 1,
                Width = 60,
                Height = 20,
            };
            SetupDisco();
            Add(graphView);
        }

        private void SetupDisco()
        {
            graphView.Reset();

            graphView.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

            var stiple = new GraphCellToRender('\u2593');

            Random r = new();
            var series = new DiscoBarSeries();
            var bars = new List<BarSeries.Bar>();
            for (int i = 0; i < 31; i++)
            {
                bars.Add(new BarSeries.Bar(null, stiple, 1));
            }

            Func<MainLoop, bool> genSample = (l) =>
            {
                bars.RemoveAt(0);
                Random random = new Random();
                int randomNumber = random.Next(1, 101);
                //int randomNumber = 10;
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView.Series.Contains(series);
            };

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), genSample);

            series.Bars = bars;

            graphView.Series.Add(series);

            // How much graph space each cell of the console depicts
            graphView.CellSize = new PointF(1, 10);
            graphView.AxisX.Increment = 0; // No graph ticks
            graphView.AxisX.ShowLabelsEvery = 0; // no labels

            graphView.AxisX.Visible = false;
            graphView.AxisY.Visible = false;

            graphView.SetNeedsDisplay();
        }
    }
}
