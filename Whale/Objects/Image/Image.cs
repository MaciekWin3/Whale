namespace Whale.Objects.Image
{
    public class Image : DockerObject
    {
        public string Id { get; set; }
        public string[]? RepoTags { get; set; }
        public string[]? RepoDigests { get; set; }
        public string? Parent { get; set; }
        public string? Comment { get; set; }
        public string? Created { get; set; }
        public string? Container { get; set; }
        public ContainerConfig? ContainerConfig { get; set; }
        public string? DockerVersion { get; set; }
        public string? Author { get; set; }
        public Config? Config { get; set; }
        public string? Architecture { get; set; }
        public string? Os { get; set; }
        public int? Size { get; set; }
        public int? VirtualSize { get; set; }
        public GraphDriver? GraphDriver { get; set; }
        public RootFS? RootFS { get; set; }
        public Metadata? Metadata { get; set; }
    }
}
