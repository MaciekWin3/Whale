namespace Whale.Models
{
    public record Image
    {
        public string? Containers { get; set; }
        public string? CreatedAt { get; set; }
        public string? CreatedSince { get; set; }
        public string? Digest { get; set; }
        public string? ID { get; set; }
        public string? Repository { get; set; }
        public string? SharedSize { get; set; }
        public string? Size { get; set; }
        public string? Tag { get; set; }
        public string? UniqueSize { get; set; }
        public string? VirtualSize { get; set; }
    }
}
