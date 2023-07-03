namespace Whale.Models.Version
{
    public record Server
    {
        public Platform? Platform { get; set; }
        public Component[]? Components { get; set; }
        public string? Version { get; set; }
        public string? ApiVersion { get; set; }
        public string? MinAPIVersion { get; set; }
        public string? GitCommit { get; set; }
        public string? GoVersion { get; set; }
        public string? Os { get; set; }
        public string? Arch { get; set; }
        public string? KernelVersion { get; set; }
        public string? BuildTime { get; set; }
    }
}
