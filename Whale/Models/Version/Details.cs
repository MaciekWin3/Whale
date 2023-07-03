namespace Whale.Models.Version
{
    public record Details
    {
        public string? ApiVersion { get; set; }
        public string? Arch { get; set; }
        public string? BuildTime { get; set; }
        public string? Experimental { get; set; }
        public string? GitCommit { get; set; }
        public string? GoVersion { get; set; }
        public string? KernelVersion { get; set; }
        public string? MinAPIVersion { get; set; }
        public string? Os { get; set; }
    }
}
