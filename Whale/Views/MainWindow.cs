using System.Diagnostics;
using System.Text;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;
using Whale.Utils;

namespace Whale.Views
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
                Height = Dim.Percent(34),
            };

            var imagesFrame = new FrameView("Images")
            {
                X = Pos.Right(tabView),
                Y = Pos.Bottom(containersFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(33),
            };

            var volumesFrame = new FrameView("Volumes")
            {
                X = Pos.Right(tabView),
                Y = Pos.Bottom(imagesFrame),
                Width = Dim.Percent(50),
                Height = Dim.Percent(33),
            };

            var textContainers = new Label()
            {
                Text = "Loading...",
            };

            var textImages = new Label()
            {
                Text = "Loading..."
            };

            var textVolumes = new Label()
            {
                Text = "Loading..."
            };

            imagesFrame.Add(textImages);
            Add(imagesFrame);
            containersFrame.Add(textContainers);
            Add(containersFrame);
            volumesFrame.Add(textVolumes);
            Add(volumesFrame);

            Task.Run(() =>
            {
                textContainers.Text = ShellCommandRunner.RunCommand("docker ps -a");
                textImages.Text = ShellCommandRunner.RunCommand("docker image ls");
                Task.Delay(2000);
                Application.Refresh();
                textVolumes.Text = ShellCommandRunner.RunCommand("docker volume ls");
            });

            Application.Refresh();

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
                Text = "",
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
                bars.Add(new BarSeries.Bar(null, stiple, CommandValidator.GetCpuUsageOfContainer("420")));
                graphView.SetNeedsDisplay();

                // while the equaliser is showing
                return graphView.Series.Contains(series);
            };

            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(1250), genSample);

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
