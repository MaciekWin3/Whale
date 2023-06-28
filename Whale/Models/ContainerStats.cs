namespace Whale.Models
{
    public class ContainerStats
    {
        public string ID { get; set; }
        public string Container { get; set; }
        public string BlockIO { get; set; }
        public string CPUPerc { get; set; }
        public string MemPerc { get; set; }
        public string MemUsage { get; set; }
        public string Name { get; set; }
        public string NetIO { get; set; }
        public string PIDs { get; set; }
    }
}
