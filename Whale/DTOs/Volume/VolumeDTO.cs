namespace Whale.DTOs.Volume
{
    public class VolumeDTO : DockerObjectDTO
    {
        public required string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Driver { get; set; }
        public LabelsDTO? Labels { get; set; }
        public string? Mountpoint { get; set; }
        public OptionsDTO? Options { get; set; }
        public string? Scope { get; set; }
    }
}
