namespace Whale.Models
{
    public class ContainerStats
    {
        public required string ContainerId { get; init; }
        public required string ContainerName { get; init; }
        public float CpuPercentage { get; init; }
        public int BlockIO { get; init; }
        public int BlockIOLimit { get; init; }
        public float MemoryPercentage { get; init; }
        public float MemoryUsage { get; init; }
        public float MemoryLimit { get; init; }
        public int Pids { get; init; }
    }
}
