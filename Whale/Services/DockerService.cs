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

        public async Task<Result<List<Container>>> GetContainerListAsync()
        {
            var result = await shellCommandRunner.RunCommandAsync("docker", "ps", "-a");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var containers = MapCommandToContainerList(result.Value.std);
                    return containers;
                }
                return Result.Fail<List<Container>>("No containers found");
            }
            return Result.Fail<List<Container>>("Command failed");
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

        private Result<List<Container>> MapCommandToContainerList(string output)
        {
            var lines = output.Trim().Split("\n").ToList();
            lines.RemoveAt(0);
            var containers = new List<Container>();
            foreach (var line in lines)
            {
                containers.Add(new Container()
                {
                    Id = line.Split(" ")[0],
                    Image = new Image() { Name = line.Split(" ")[1] },
                    Command = line.Split(" ")[2],
                    //CreatedDate = DateTime.Parse(line.Split(" ")[3]),
                    Status = new Status() { State = line.Split(" ")[4] },
                });
            }
            return Result.Ok(containers);
        }
    }
}
