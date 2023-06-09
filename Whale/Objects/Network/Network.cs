namespace Whale.Objects.Network
{

    public class Network : DockerObject
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Created { get; set; }
        public string? Scope { get; set; }
        public string? Driver { get; set; }
        public bool? EnableIPv6 { get; set; }
        public IPAM? IPAM { get; set; }
        public bool? Internal { get; set; }
        public bool? Attachable { get; set; }
        public bool? Ingress { get; set; }
        public ConfigFrom? ConfigFrom { get; set; }
        public bool? ConfigOnly { get; set; }
        public Containers? Containers { get; set; }
        public Options? Options { get; set; }
        public Labels? Labels { get; set; }
    }
}
