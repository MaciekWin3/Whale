using System.Diagnostics;
using System.Text;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace Whale
{
    public class MainWindow : Window
    {
        GraphView graphView;
        public MainWindow() : base("Whale Dashboard")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
            InitWindow();
        }

        public void InitWindow()
        {
            var tabView = new TabView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill(),
            };
            var containersFrame = new FrameView("Containers")
            {
                X = Pos.Right(tabView),
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            var imagesFrame = new FrameView("Images")
            {
                X = Pos.Right(tabView),
                Y = Pos.Bottom(containersFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            var text = new Label()
            {
                Text = "XDDD",
            };

            var text2 = new Label("")
            {
                Text = "Xddd"
            };

            imagesFrame.Add(text);
            Add(imagesFrame);
            containersFrame.Add(text2);
            Add(containersFrame);

            Task.Run(() =>
            {
                text.Text = ShellCommandRunner.RunCommand("docker ps -a");
                Application.Refresh();
            });

            Task.Run(() =>
            {
                text2.Text = ShellCommandRunner.RunCommand("docker images");
                Application.Refresh();
            });

            tabView.AddTab(new TabView.Tab("Chart", Bar()), false);
            tabView.AddTab(new TabView.Tab("Images", ImagesView()), false);
            tabView.AddTab(new TabView.Tab("Interative Tab", GetInteractiveTab()), false);
            Add(tabView);
        }

        private View Bar()
        {
            var imagesView = new View()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            graphView = new GraphView()
            {
                X = 1,
                Y = 1,
                Width = 60,
                Height = 20,
            };
            SetupDisco();
            imagesView.Add(graphView);
            return imagesView;
        }

        private View ImagesView()
        {
            var imagesView = new View()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            var imagesFrame = new FrameView("Images");

            var text = new Label()
            {
                Text = "XDDD",
            };
            imagesFrame.Add(text);
            imagesView.Add(imagesFrame);
            imagesView.Add(text);

            Application.MainLoop.Invoke(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo("ping", "wp.pl");
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                Process proc = new Process();
                proc.StartInfo = psi;
                StringBuilder sb = new StringBuilder();
                proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => OnOutputDataReceived(sender, e, sb));

                // Start the process and begin reading its output
                proc.Start();
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                text.Text += sb.ToString();
                //text.Text = ShellCommandRunner.RunCommand("docker volume ls");
                Application.Refresh();
            });

            return imagesView;
        }
        static void OnOutputDataReceived(object sender, DataReceivedEventArgs e, StringBuilder sb)
        {
            // Append each line of output to the StringBuilder object
            if (!string.IsNullOrEmpty(e.Data))
            {
                sb.AppendLine(e.Data);
            }
        }

        private View GetInteractiveTab()
        {

            var interactiveTab = new View()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            var lblName = new Label("Name:");
            interactiveTab.Add(lblName);

            var tbName = new TextField()
            {
                X = Pos.Right(lblName),
                Width = 10
            };
            interactiveTab.Add(tbName);

            var lblAddr = new Label("Address:")
            {
                Y = 1
            };
            interactiveTab.Add(lblAddr);

            var tbAddr = new TextField()
            {
                X = Pos.Right(lblAddr),
                Y = 1,
                Width = 10
            };
            interactiveTab.Add(tbAddr);

            return interactiveTab;
        }

        private void SetupDisco()
        {
            graphView.Reset();

            graphView.GraphColor = Application.Driver.MakeAttribute(Color.White, Color.Black);

            var stiple = new GraphCellToRender('\u2593');

            Random r = new Random();
            var series = new DiscoBarSeries();
            var bars = new List<BarSeries.Bar>();
            for (int i = 0; i < 31; i++)
            {
                bars.Add(new BarSeries.Bar(null, stiple, 1));
            }

            Func<MainLoop, bool> genSample = (l) =>
            {
                bars.RemoveAt(0);
                bars.Add(new BarSeries.Bar(null, stiple, GetCpuUsageWindows()));
                graphView.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView.Series.Contains(series);
            };

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(250), genSample);

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

        public float GetCpuUsageWindows()
        {
            Process process = Process.GetCurrentProcess();
            float cpuUsage = (process.TotalProcessorTime.Ticks / (float)Stopwatch.Frequency / Environment.ProcessorCount) * 100;
            return cpuUsage;
        }

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
            protected override void DrawBarLine(GraphView graph, Terminal.Gui.Point start, Terminal.Gui.Point end, Bar beingDrawn)
            {
                var driver = Application.Driver;

                int x = start.X;
                for (int y = end.Y; y <= start.Y; y++)
                {

                    var height = graph.ScreenToGraphSpace(x, y).Y;

                    if (height >= 85)
                    {
                        driver.SetAttribute(red);
                    }
                    else if (height >= 66)
                    {
                        driver.SetAttribute(brightred);
                    }
                    else if (height >= 45)
                    {
                        driver.SetAttribute(brightyellow);
                    }
                    else if (height >= 25)
                    {
                        driver.SetAttribute(brightgreen);
                    }
                    else
                    {
                        driver.SetAttribute(green);
                    }

                    graph.AddRune(x, y, beingDrawn.Fill.Rune);
                }
            }
        }
    }
}
