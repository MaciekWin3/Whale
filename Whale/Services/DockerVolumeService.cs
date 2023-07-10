using Whale.Models;
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
            var result = await shellCommandRunner.RunCommandAsync("docker",
                new[] { "volume", "ls", "--format", "json" }, token);
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
    }
}
