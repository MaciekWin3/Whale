namespace Whale.Objects.Volume
{
    public class Volume : DockerObject
    {
        public DateTime? CreatedAt { get; set; }
        public string? Driver { get; set; }
        public Labels? Labels { get; set; }
        public string? Mountpoint { get; set; }
        public string? Name { get; set; }
        public Options? Options { get; set; }
        public string? Scope { get; set; }
    }
}
