namespace Whale.Models.DockerVersion.DockerVersion
{
    public record Client
    {
        public string CloudIntegration { get; set; }
        public string Version { get; set; }
        public string ApiVersion { get; set; }
        public string DefaultAPIVersion { get; set; }
        public string GitCommit { get; set; }
        public string GoVersion { get; set; }
        public string Os { get; set; }
        public string Arch { get; set; }
        public string BuildTime { get; set; }
        public string Context { get; set; }
    }
}
