﻿using System.Text.Json;
using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public class DockerService : IDockerService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }
        public DockerService(ShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<List<Container>>> GetContainerListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker",
                new[] { "container", "ls", "--all", "--format", "json" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = Mapper.MapCommandToContainerList(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<Container>>("No containers found");
            }
            return Result.Fail<List<Container>>("Command failed");
        }

        public async Task<Result<List<Volume>>> GetVolumeListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "volume", "ls" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var volumes = Mapper.MapCommandToVolumeList(result.Value.std);
                    return volumes;
                }
                return Result.Fail<List<Volume>>("No containers found");
            }
            return Result.Fail<List<Volume>>("Command failed");
        }

        public async Task<Result<List<Image>>> GetImageListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "images" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var images = Mapper.MapCommandToImageList(result.Value.std);
                    return images;
                }
                return Result.Fail<List<Image>>("No images found");
            }
            return Result.Fail<List<Image>>("Command failed");
        }

        public async Task<Result<(string std, string err)>> GetContainerLogsAsync(string containerId, CancellationToken token = default)
        {
            // docker logs -f ???
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "logs", containerId }, default);
            if (result.IsSuccess)
            {
                return result.Value;
            }
            return Result.Fail<(string std, string err)>("Command failed");
        }

        //public async Task<Result> GetContainerStatsAsync(string containerId, CancellationToken token = default)
        //{
        //    var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "stats", containerId }, default);
        //    if (result.IsSuccess)
        //    {
        //        return result.Value;
        //    }
        //    return Result.Fail<(string std, string err)>("Command failed");
        //}

        public async Task<Result> CreateContainerAsync(List<string> arguments)
        {
            var commandParameters = arguments.Prepend("create").ToArray();
            var result = await shellCommandRunner.RunCommandAsync("docker", commandParameters, default);
            if (result.IsSuccess)
            {
                return result;
            }
            return Result.Fail<(string std, string err)>("Command failed");
        }

        public async Task<Result<string>> RunCommandInsideDockerContainerAsync(string containerId, string command, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "exec", containerId, command }, token);
            if (result.IsSuccess)
            {
                return Result.Ok(result.Value.std);
            }
            return Result.Fail<string>("Command failed");
        }

        public async Task<Result<T>> GetDockerObjectInfoAsync<T>(string id, CancellationToken token = default)
        {
            var result = await shellCommandRunner
                .RunCommandAsync("docker", new[] { "inspect", id }, token);

            if (result.IsFailure)
            {
                return Result.Fail<T>("Command failed");
            }

            JsonElement x = JsonDocument.Parse(result.Value.std).RootElement;

            JsonElement firstElement = x
                .EnumerateArray()
                .FirstOrDefault();

            var rawText = firstElement.GetRawText();
            var dockerObject = JsonSerializer.Deserialize<T>(rawText, new JsonSerializerOptions() { });

            if (dockerObject is null)
            {
                return Result.Fail<T>("Failed to deserialize object");
            }

            return Result.Ok(dockerObject);
        }

        public async Task<Result<bool>> CheckIfDockerDaemonIsRunningAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", new[] { "ps" }, token);
            if (result.IsSuccess)
            {
                return Result.Ok(true);
            }
            return Result.Fail<bool>("Docker daemon is not running");
        }
    }
}
