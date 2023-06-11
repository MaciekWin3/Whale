namespace Whale.Models
{
    public record VolumeDTO
    {
        public required string Name { get; set; }
        public string? Driver { get; set; }
    }
}
