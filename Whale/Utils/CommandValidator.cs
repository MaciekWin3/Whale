namespace Whale.Utils
{
    public static class CommandValidator
    {
        public static float GetCpuUsageOfContainer(string containerId)
        {
            //string output = ShellCommandRunner.RunCommand($"docker stats --no-stream {containerId}");
            //string[] lines = output.Split('\n');
            //foreach (string line in lines)
            //{
            //    if (line.StartsWith("240"))
            //    {
            //        string[] columns = line.Split(new char[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
            //        string cpuUsage = columns[2].TrimEnd('%');
            //        float cpuUsageFloat = float.Parse(cpuUsage);
            //        return cpuUsageFloat;
            //    }
            //}
            //return 0.0f;
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            return randomNumber;
        }
    }
}
