namespace Whale.DTOs.Container
{
    public class BridgeDTO
    {
        public object? IPAMConfig { get; set; }
        public object? Links { get; set; }
        public object? Aliases { get; set; }
        public string? NetworkID { get; set; }
        public string? EndpointID { get; set; }
        public string? Gateway { get; set; }
        public string? IPAddress { get; set; }
        public int IPPrefixLen { get; set; }
        public string? IPv6Gateway { get; set; }
        public string? GlobalIPv6Address { get; set; }
        public int GlobalIPv6PrefixLen { get; set; }
        public string? MacAddress { get; set; }
        public object? DriverOpts { get; set; }
    }
}
