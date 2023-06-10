namespace Whale.Objects.Volume
{
    public class Volume : DockerObject
    {
        public required string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Driver { get; set; }
        public Labels? Labels { get; set; }
        public string? Mountpoint { get; set; }
        public Options? Options { get; set; }
        public string? Scope { get; set; }
    }
}
