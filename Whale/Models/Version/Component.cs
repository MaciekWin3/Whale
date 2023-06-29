namespace Whale.Models.Version
{
    public record Component
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Details Details { get; set; }
    }
}
