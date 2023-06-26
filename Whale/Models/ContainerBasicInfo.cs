namespace Whale.Models
{
    public record ContainerBasicInfo
    {
        public required string Id { get; set; }
        public required string Image { get; set; }
        public required string Command { get; set; }
        public required string Created { get; set; }
        public required string Status { get; set; }
        public required string Ports { get; set; }
        public required string Names { get; set; }
    }
}
