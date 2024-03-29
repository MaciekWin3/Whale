﻿using Whale.Models;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Services
{
    public sealed class DockerVolumeService : IDockerVolumeService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerVolumeService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<List<Volume>>> GetVolumeListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker volume ls --format json", token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var volumes = Mapper.MapCommandToDockerObjectList<Volume>(result.Value.std);
                    return volumes;
                }
                return Result.Fail<List<Volume>>("Container not found");
            }
            return Result.Fail<List<Volume>>("Command failed");
        }

        public async Task<Result<List<Container>>> GetVolumesContainersListAsync(string containerId, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync($"docker ps -a --format json --filter volume={containerId}", token);

            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = Mapper.MapCommandToDockerObjectList<Container>(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<Container>>("Container not found");
            }
            return Result.Fail<List<Container>>("Command failed");
        }

        public async Task<Result> DeleteVolumeAsync(string volumeId, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync($"docker volume remove {volumeId}", token);
            if (result.IsSuccess)
            {
                return Result.Ok();
            }
            return Result.Fail("Command failed");
        }
    }
}
