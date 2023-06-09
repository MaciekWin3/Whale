using System.Text.Json;
using Whale.Models;
using Whale.Objects;
using Whale.Objects.Container;
using Whale.Objects.Image;
using Whale.Objects.Network;
using Whale.Objects.Volume;
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
                    var containers = MapCommandToContainerList(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<ContainerDTO>>("No containers found");
            }
            return Result.Fail<List<ContainerDTO>>("Command failed");
        }

        public async Task<Result<List<string>>> GetImageListAsync()
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", "images");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var lines = result.Value.std.Split("\n");
                    var images = new List<string>();
                    foreach (var line in lines)
                    {
                        if (line.Length > 0)
                        {
                            images.Add(line);
                        }
                    }
                    return Result.Ok(images);
                }
                return Result.Fail<List<string>>("No images found");
            }
            return Result.Fail<List<string>>("Command failed");
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

        public Type GetDockerObject(DockerObjectType type) => type switch
        {
            DockerObjectType.Container => typeof(Container),
            DockerObjectType.Image => typeof(Image),
            DockerObjectType.Network => typeof(Network),
            DockerObjectType.Volume => typeof(Volume),
            _ => throw new NotImplementedException(),
        };

        private List<string> PrepareOutput(string output)
        {
            var lines = output
                .Trim()
                .Split("\n")
                .ToList();

            lines.RemoveAt(0);

            return lines;
        }

        private Result<List<ContainerDTO>> MapCommandToContainerList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<ContainerDTO>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new ContainerDTO()
                {
                    Id = values[0],
                    //Image = new Image() { Name = values[1] },
                    Command = values[2],
                    //CreatedDate = DateTime.Now,
                });
            }
            return Result.Ok(containers);
        }
    }
}


public class Rootobject
{
    public Volume[] Property1 { get; set; }
}
