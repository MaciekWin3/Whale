using System.Text.Json;
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

        public async Task<Result<List<ContainerDTO>>> GetContainerListAsync()
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", "ps", "-a");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = Mapper.MapCommandToContainerList(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<ContainerDTO>>("No containers found");
            }
            return Result.Fail<List<ContainerDTO>>("Command failed");
        }

        public async Task<Result<List<VolumeDTO>>> GetVolumeListAsync()
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", "volume", "ls");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var volumes = Mapper.MapCommandToVolumeList(result.Value.std);
                    return volumes;
                }
                return Result.Fail<List<VolumeDTO>>("No containers found");
            }
            return Result.Fail<List<VolumeDTO>>("Command failed");
        }

        public async Task<Result<List<ImageDTO>>> GetImageListAsync()
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", "images");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var images = Mapper.MapCommandToImageList(result.Value.std);
                    return images;
                }
                return Result.Fail<List<ImageDTO>>("No images found");
            }
            return Result.Fail<List<ImageDTO>>("Command failed");
        }

        public async Task<Result<T>> GetDockerObjectInfoAsync<T>(string id)
        {
            var result = await shellCommandRunner
                .RunCommandAsync("docker", "inspect", id);

            if (result.IsFailure)
            {
                return Result.Fail<T>("Command failed");
            }

            //string jsonString = JsonSerializer.Serialize(result.Value.std);
            JsonElement x = JsonDocument.Parse(result.Value.std, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }).RootElement;

            JsonElement firstElement = x
                .EnumerateArray()
                .FirstOrDefault();

            return Result.Ok(JsonSerializer.Deserialize<T>(firstElement));
        }
    }
}
