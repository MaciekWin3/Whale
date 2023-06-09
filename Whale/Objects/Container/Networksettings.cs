namespace Whale.Objects.Container
{
    public class Networksettings
    {
        public string Bridge { get; set; }
        public string SandboxID { get; set; }
        public bool HairpinMode { get; set; }
        public string LinkLocalIPv6Address { get; set; }
        public int LinkLocalIPv6PrefixLen { get; set; }
        public Ports Ports { get; set; }
        public string SandboxKey { get; set; }
        public object SecondaryIPAddresses { get; set; }
        public object SecondaryIPv6Addresses { get; set; }
        public string EndpointID { get; set; }
        public string Gateway { get; set; }
        public string GlobalIPv6Address { get; set; }
        public int GlobalIPv6PrefixLen { get; set; }
        public string IPAddress { get; set; }
        public int IPPrefixLen { get; set; }
        public string IPv6Gateway { get; set; }
        public string MacAddress { get; set; }
        public Networks Networks { get; set; }
    }
}
