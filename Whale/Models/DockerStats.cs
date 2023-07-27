namespace Whale.Models
{
    public record DockerStats
    {
        // Disk 
        public int BlockInput { get; set; }
        public int BlockOutput { get; set; }
        // CPU
        public float CPUPerc { get; set; }
        // Ram
        public float MemPerc { get; set; }
        public float MemUsage { get; set; }
        public float MemLimit { get; set; }
        // Net
        public float NetInput { get; set; }
        public float NetOutput { get; set; }
        // Process
        public int PIDs { get; set; }
    }
}
