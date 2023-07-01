using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;
using Whale.Services;

namespace Whale.Windows.Single.ContainerTabs
{

    public sealed class ContainerStatsWindow : Toplevel
    {
        GraphView graphView = null!;
        GraphView graphView2 = null!;
        GraphView graphView3 = null!;
        GraphView graphView4 = null!;
        private readonly IShellCommandRunner shellCommandRunner;
        private readonly IDockerService dockerService;
        public string ContainerId { get; set; }
        public ContainerStatsWindow(string containerId) : base()
        {
            ContainerId = containerId;
            shellCommandRunner = new ShellCommandRunner();
            dockerService = new DockerService(shellCommandRunner);
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
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            graphView2 = new GraphView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            graphView3 = new GraphView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            graphView4 = new GraphView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            // hi i want 4 framesview two on top two on bottom
            var cpuFrame = new FrameView("CPU")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            var memFrame = new FrameView("Memory")
            {
                X = Pos.Right(cpuFrame),
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            var diskFrame = new FrameView("Disk")
            {
                X = 0,
                Y = Pos.Bottom(cpuFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            var netFrame = new FrameView("Network")
            {
                X = Pos.Right(diskFrame),
                Y = Pos.Bottom(memFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };
            // GraphView graphView = new GraphView()

            //cpuFrame.Add(graphView);
            //memFrame.Add(graphView2);
            //diskFrame.Add(graphView3);
            //netFrame.Add(graphView4);

            Add(cpuFrame, memFrame, diskFrame, netFrame);

            var cpuLabel = new Label("CPU Usage")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                TextAlignment = TextAlignment.Centered,
                VerticalTextAlignment = VerticalTextAlignment.Middle
            };
            cpuFrame.Add(cpuLabel);

            var memLabel = new Label("Memory Usage")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                TextAlignment = TextAlignment.Centered,
                VerticalTextAlignment = VerticalTextAlignment.Middle
            };
            memFrame.Add(memLabel);

            var diskLabel = new Label("Disk Usage")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                TextAlignment = TextAlignment.Centered,
                VerticalTextAlignment = VerticalTextAlignment.Middle
            };

            diskFrame.Add(diskLabel);

            var netLabel = new Label("Network Usage")
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                TextAlignment = TextAlignment.Centered,
                VerticalTextAlignment = VerticalTextAlignment.Middle
            };

            netFrame.Add(netLabel);

            Application.MainLoop.Invoke(async () =>
            {
                while (true)
                {
                    var stats = await dockerService.GetContainerStatsAsync(ContainerId);
                    cpuLabel.Text = stats.Value.CPUPerc.ToString();
                    memLabel.Text = stats.Value.MemPerc.ToString();
                    diskLabel.Text = stats.Value.BlockIO.ToString();
                    netLabel.Text = stats.Value.NetIO.ToString();
                }
            });

            // now i want text inside each frameview that is centerd bothj verticaly and horizontaly
            //cpuFrame.Add(new Label("CPU USage") { VerticalTextAlignment = VerticalTextAlignment.Middle });
            //SetupDisco();
            //SetupDisco2();
            //SetupDisco3();
            //SetupDisco4();
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

            bool genSample(MainLoop l)
            {
                bars.RemoveAt(0);
                Random random = new();
                int randomNumber = random.Next(1, 101);
                //int randomNumber = 10;
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView.Series.Contains(series);
            }

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

        private void SetupDisco2()
        {
            graphView2.Reset();

            graphView2.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

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
                Random random = new();
                int randomNumber = random.Next(1, 101);
                //int randomNumber = 10;
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView2.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView2.Series.Contains(series);
            };

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), genSample);

            series.Bars = bars;

            graphView2.Series.Add(series);

            // How much graph space each cell of the console depicts
            graphView2.CellSize = new PointF(1, 10);
            graphView2.AxisX.Increment = 0; // No graph ticks
            graphView2.AxisX.ShowLabelsEvery = 0; // no labels

            graphView2.AxisX.Visible = false;
            graphView2.AxisY.Visible = false;

            graphView2.SetNeedsDisplay();
        }

        // do same for other two graphs only change graphview
        private void SetupDisco3()
        {
            graphView3.Reset();
            graphView3.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

            var stiple = new GraphCellToRender('\u2593');

            Random r = new();
            var series = new DiscoBarSeries();
            var bars = new List<BarSeries.Bar>();
            for (int i = 0; i < 31; i++)
            {
                bars.Add(new BarSeries.Bar(null, stiple, 1));
            }

            bool genSample(MainLoop l)
            {
                bars.RemoveAt(0);
                Random random = new();
                int randomNumber = random.Next(1, 101);
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView3.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView3.Series.Contains(series);
            }

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), genSample);

            series.Bars = bars;

            graphView3.Series.Add(series);

            // How much graph space each cell of the console depicts
            graphView3.CellSize = new PointF(1, 10);
            graphView3.AxisX.Increment = 0; // No graph ticks
            graphView3.AxisX.ShowLabelsEvery = 0; // no labels

            graphView3.AxisX.Visible = true;
            graphView3.AxisY.Visible = true;

            graphView3.SetNeedsDisplay();
        }

        private void SetupDisco4()
        {
            graphView4.Reset();
            graphView4.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

            var stiple = new GraphCellToRender('\u2593');

            Random r = new();
            var series = new DiscoBarSeries();
            var bars = new List<BarSeries.Bar>();
            for (int i = 0; i < 31; i++)
            {
                bars.Add(new BarSeries.Bar(null, stiple, 1));
            }

            bool genSample(MainLoop l)
            {
                bars.RemoveAt(0);
                Random random = new();
                int randomNumber = random.Next(1, 101);
                //int randomNumber = 10;
                bars.Add(new BarSeries.Bar(null, stiple, randomNumber));
                graphView4.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView4.Series.Contains(series);
            }

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), genSample);

            series.Bars = bars;

            graphView4.Series.Add(series);

            // How much graph space each cell of the console depicts
            graphView4.CellSize = new PointF(1, 10);
            graphView4.AxisX.Increment = 0; // No graph ticks
            graphView4.AxisX.ShowLabelsEvery = 0; // no labels

            graphView4.AxisX.Visible = false;
            graphView4.AxisY.Visible = false;

            graphView4.SetNeedsDisplay();
        }

    }
}
