namespace Whale.Objects.Image
{
    public class ImageDTO : DockerObjectDTO
    {
        public string? Id { get; set; }
        public string[]? RepoTags { get; set; }
        public string[]? RepoDigests { get; set; }
        public string? Parent { get; set; }
        public string? Comment { get; set; }
        public string? Created { get; set; }
        public string? Container { get; set; }
        public ContainerConfigDTO? ContainerConfig { get; set; }
        public string? DockerVersion { get; set; }
        public string? Author { get; set; }
        public ConfigDTO? Config { get; set; }
        public string? Architecture { get; set; }
        public string? Os { get; set; }
        public int? Size { get; set; }
        public int? VirtualSize { get; set; }
        public GraphDriverDTO? GraphDriver { get; set; }
        public RootFSDTO? RootFS { get; set; }
        public MetadataDTO? Metadata { get; set; }
    }
}
