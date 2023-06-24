using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public static class Mapper
    {
        public static Result<List<Container>> MapCommandToContainerList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<Container>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new Container()
                {
                    Id = values[0],
                    //Image = new Image() { Name = values[1] },
                    Command = values[2],
                    //CreatedDate = DateTime.Now,
                });
            }
            return Result.Ok(containers);
        }

        public static Result<List<Image>> MapCommandToImageList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<Image>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new Image()
                {
                    Name = values[0],
                });
            }
            return Result.Ok(containers);
        }

        public static Result<List<Volume>> MapCommandToVolumeList(string output)
        {
            var lines = PrepareOutput(output);
            var containers = new List<Volume>();
            foreach (var line in lines)
            {
                var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                containers.Add(new Volume()
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
