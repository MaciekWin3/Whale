using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public static class Mapper
    {
        public static Result<List<ContainerDTO>> MapCommandToContainerList(string output)
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

        public static Result<List<ImageDTO>> MapCommandToImageList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<ImageDTO>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new ImageDTO()
                {
                    Name = values[0],
                });
            }
            return Result.Ok(containers);
        }

        public static Result<List<VolumeDTO>> MapCommandToVolumeList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<VolumeDTO>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new VolumeDTO()
                {
                    Driver = values[0],
                    Name = values[1],
                });
            }
            return Result.Ok(containers);
        }

        private static List<string> PrepareOutput(string output)
        {
            var lines = output
                .Trim()
                .Split("\n")
                .ToList();

            lines.RemoveAt(0);

            return lines;
        }
    }
}
