namespace Whale.Models
{
    public record Volume
    {
        public string? Availability { get; set; }
        public string? Driver { get; set; }
        public string? Group { get; set; }
        public string? Labels { get; set; }
        public string? Links { get; set; }
        public string? Mountpoint { get; set; }
        public string? Name { get; set; }
        public string? Scope { get; set; }
        public string? Size { get; set; }
        public string? Status { get; set; }
    }
}
