using Whale.Models;
using Whale.Services.Interfaces;
using Whale.Utils;

namespace Whale.Services
{
    public class DockerImageService : IDockerImageService
    {
        private readonly IShellCommandRunner shellCommandRunner;
        public DockerImageService(IShellCommandRunner shellCommandRunner)
        {
            this.shellCommandRunner = shellCommandRunner;
        }

        public async Task<Result<List<Image>>> GetImageListAsync(CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker",
                new[] { "image", "ls", "--all", "--format", "json" }, token);
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var images = Mapper.MapCommandToDockerObjectList<Image>(result.Value.std);
                    return images;
                }
                return Result.Fail<List<Image>>("No images found");
            }
            return Result.Fail<List<Image>>("Command failed");
        }

        public async Task<Result<List<ImageLayer>>> GetImageLayersAsync(string containerId, CancellationToken token = default)
        {
            var result = await shellCommandRunner.RunCommandAsync("docker",
               new[] { "image", "history", containerId, "--format", "json" }, token);

            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var imagesLayers = Mapper.MapCommandToDockerObjectList<ImageLayer>(result.Value.std);
                    return imagesLayers;
                }
                return Result.Fail<List<ImageLayer>>("No images found");
            }
            return Result.Fail<List<ImageLayer>>("Command failed");

        }
    }
}
