﻿using Terminal.Gui;
using Whale.Components;
using Whale.Models;
using Whale.Objects.Volume;
using Whale.Services;
using Whale.Utils;
using Whale.Windows.Single;

namespace Whale.Windows.Lists
{
    public class VolumeListWindow : Window
    {
        readonly Action<int, int> showContextMenu;
        private readonly ShellCommandRunner shellCommandRunner = new();
        private readonly IDockerService dockerService =
            new DockerService(new ShellCommandRunner());
        public VolumeListWindow(Action<int, int> showContextMenu) : base()
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
                Width = Dim.Fill(),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

            // Listener
            Application.MainLoop.Invoke(async () =>
            {
                string cache = string.Empty;
                while (true)
                {
                    Result<(string std, string error)> result
                        = await shellCommandRunner.RunCommandAsync("docker", "volume", "ls");
                    if (!result.IsSuccess)
                    {
                        continue;
                    }
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
            });

            listview.KeyDown += (e) =>
            {
                if (e.KeyEvent.Key == Key.m)
                {
                    showContextMenu.Invoke(1, 1);
                }
            };

            listview.OpenSelectedItem += async (e) =>
            {
                var name = e.Value.ToString();
                if (name is not null)
                {
                    var x = await dockerService.GetDockerObjectInfoAsync<Volume>(name);
                    Application.Top.RemoveAll();
                    var volumeWindow = new VolumeWindow(name);
                    Application.Top.Add(volumeWindow);
                    Application.Top.Add(MenuBarX.CreateMenuBar());
                    Application.Refresh();
                }
            };
            Add(listview);
        }
    }
}
