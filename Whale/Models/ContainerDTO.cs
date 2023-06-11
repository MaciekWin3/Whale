namespace Whale.Models
{
    public record ContainerDTO
    {
        public required string Id { get; set; }
        public ImageDTO Image { get; set; }
        public required string Command { get; set; }
        public DateTime CreatedDate { get; set; }
        public StatusDTO Status { get; set; }
        public List<Port> Ports { get; set; }
        public List<string> Names { get; set; }
    }
}
