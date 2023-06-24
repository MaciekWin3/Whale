namespace Whale.Models
{
    public record Volume
    {
        public required string Name { get; set; }
        public string? Driver { get; set; }
    }
}
