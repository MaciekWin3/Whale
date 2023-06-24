namespace Whale.Models
{
    public record Container
    {
        public required string Id { get; set; }
        public Image Image { get; set; }
        public required string Command { get; set; }
        public DateTime CreatedDate { get; set; }
        public Status Status { get; set; }
        public List<Port> Ports { get; set; }
        public List<string> Names { get; set; }
    }
}
