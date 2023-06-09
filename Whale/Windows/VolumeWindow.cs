﻿using Terminal.Gui;
using Whale.Models;
using Whale.Objects.Volume;
using Whale.Services;
using Whale.Utils;

namespace Whale.Windows
{
    public class VolumeWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly ShellCommandRunner shellCommandRunner = new ShellCommandRunner();
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public VolumeWindow(Action<int, int> showContextMenu) : base()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            Border = new Border()
            {
                BorderStyle = BorderStyle.None,
            };
            this.showContextMenu = showContextMenu;
            InitView();
        }

        public void InitView()
        {
            var items = new List<string>() { };

            Result<List<VolumeDTO>> images;

            var listview = new ListView(items)
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(2),
                Width = Dim.Percent(40),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

            Application.MainLoop.Invoke(async () =>
            {
                string cache = "";
                while (true)
                {
                    Result<(string std, string error)> result = await shellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                    if (result.IsSuccess)
                    {
                        if (cache != result.Value.std)
                        {
                            cache = result.Value.std;
                            images = await dockerService.GetVolumeListAsync();
                            var cont = images.Value?.Select(x => x.Name.ToString()).ToList();
                            listview.RemoveAll();
                            listview.SetSource(cont);
                        }
                        else
                        {
                            cache = result.Value.std;
                        }
                    }
                }
            });

            //Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(2), genSample);
            // Hi i want to refresh app every two seconds
            // var timer = new Timer(TimeSpan.FromSeconds(2));

            listview.KeyDown += (KeyEventEventArgs e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    showContextMenu.Invoke(1, 1);
                }
            };
            listview.OpenSelectedItem += async (ListViewItemEventArgs e) =>
            {
                var name = e.Value.ToString();
                var x = await dockerService.GetDockerObjectInfoAsync<Volume>(name);
                MessageBox.Query(50, 7, name, x.Value.Mountpoint.ToString(), "Ok");
            };
            Add(listview);
        }
    }
}
