using CliWrap;
using CliWrap.EventStream;
using System.Diagnostics;
using System.Text;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using Whale.Components;

namespace Whale.Views
{
    public class MainWindow : Window
    {
        GraphView graphView = null!;
        protected MainWindow() : base("Whale Dashboard")
        {
            X = 0;
            Y = 1;
            Width = Dim.Fill();
            Height = Dim.Fill();
        }

        //public static async Task<Window> CreateAsync()
        public static Window CreateAsync()
        {
            var window = new MainWindow();
            //await window.InitWindow();
            window.InitWindow();
            Application.Refresh();
            return window;
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


            Application.MainLoop.Invoke(async () =>
            {
                var x = await ShellCommandRunner.RunCommandAsync("docker", "image", "ls");
                textImages.Text = x.Value.std;
                //var y = await ShellCommandRunner.RunCommandAsync("docker", "container", "ls");
                var z = await ShellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                textVolumes.Text = z.Value.std;
                //Application.Refresh();
            });

            Application.MainLoop.Invoke(async () =>
            {
                var cmd = Cli.Wrap("ping").WithArguments("wp.pl");
                await foreach (var cmdEvent in cmd.ListenAsync())
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            //_output.WriteLine($"Process started; ID: {started.ProcessId}");
                            textVolumes.Clear();
                            break;
                        case StandardOutputCommandEvent stdOut:
                            //_output.WriteLine($"Out> {stdOut.Text}");
                            textVolumes.Text += $"{stdOut.Text}\n";
                            break;
                        case StandardErrorCommandEvent stdErr:
                            //_output.WriteLine($"Err> {stdErr.Text}");
                            break;
                        case ExitedCommandEvent exited:
                            //_output.WriteLine($"Process exited; Code: {exited.ExitCode}");
                            break;
                    }
                }
                //Application.Refresh();
            });


            // Tabs
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

        //private async Task<View> ImagesView()
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

            Application.MainLoop.Invoke(async () =>
            {
                var x = await ShellCommandRunner.RunCommandAsync("ping", "-n", "20", "wp.pl");
                text.Text += x.Value.std;
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
