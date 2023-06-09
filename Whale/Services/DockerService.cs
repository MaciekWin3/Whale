using Whale.Utils;

namespace Whale.Services
{
    public class DockerService
    {
        public async Task<Result<List<string>>> GetContainerListAsync()
        {
            var result = await ShellCommandRunner.RunCommandAsync("docker", "ps", "-a");
            if (result.IsSuccess)
            {
                if (result.Value.std.Length > 0)
                {
                    var lines = result.Value.std.Split("\n");
                    var containers = new List<string>();
                    foreach (var line in lines)
                    {
                        if (line.Length > 0)
                        {
                            containers.Add(line);
                        }
                    }
                    return Result.Ok(containers);
                }
                return Result.Fail<List<string>>("No containers found");
            }
            return Result.Fail<List<string>>("Command failed");
        }
    }
}
