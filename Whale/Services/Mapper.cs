using System.Text.Json;
using Whale.Utils;

namespace Whale.Services
{
    public static class Mapper
    {
        public static Result<List<T>> MapCommandToDockerObjectList<T>(string output)
        {
            var dockerObjectList = new List<T>();
            var jsonObjects = output.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var jsonObject in jsonObjects)
            {
                var dockerObject = JsonSerializer.Deserialize<T>(jsonObject);
                if (dockerObject is not null)
                {
                    dockerObjectList.Add(dockerObject);
                }
            }

            if (dockerObjectList is null)
            {
                return Result.Fail<List<T>>("Failed to deserialize object");
            }
            return Result.Ok(dockerObjectList);
        }

        public static Result<T> MapCommandToDockerObject<T>(string output)
        {
            var dockerObject = JsonSerializer.Deserialize<T>(output);
            if (dockerObject is null)
            {
                return Result.Fail<T>("Failed to deserialize object");
            }
            return Result.Ok(dockerObject);
        }
    }
}
