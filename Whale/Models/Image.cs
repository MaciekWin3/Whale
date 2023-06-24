namespace Whale.Models
{
    public record Image
    {
        public required string Name { get; set; }
        public string? Tag { get; set; }
        public string? Command { get; set; }
        public string? CreatedDate { get; set; }
        public string? Status { get; set; }
        public string? Ports { get; set; }
        public string? Names { get; set; }
    }
}
