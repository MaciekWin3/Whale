using System.Text.Json;
using Whale.Models;
using Whale.Utils;

namespace Whale.Services
{
    public static class Mapper
    {
        //public static Result<List<ContainerBasicInfo>> MapCommandToContainerList(string output)
        //{
        //    var lines = PrepareOutput(output);
        //    var containers = new List<ContainerBasicInfo>();
        //    foreach (var line in lines)
        //    {
        //        var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //        containers.Add(new ContainerBasicInfo()
        //        {
        //            Id = values[0],
        //            Image = values[1],
        //            Command = values[2],
        //            Created = $"{values[3]} {values[4]} {values[5]}",
        //            Status = $"{values[6]} {values[7]} {values[8]} {values[9]} {values[10]}",
        //            Ports = values[11] ?? "",
        //            Names = values.LastOrDefault() ?? "",
        //        });
        //    }
        //    return Result.Ok(containers);
        //}

        public static Result<List<Container>> MapCommandToContainerList(string output)
        {
            var containers = new List<Container>();
            var jsonObjects = output.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var jsonObject in jsonObjects)
            {
                var container = JsonSerializer.Deserialize<Container>(jsonObject);
                if (container is not null)
                {
                    containers.Add(container);
                }
            }

            if (containers is null)
            {
                return Result.Fail<List<Container>>("Failed to deserialize object");
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
