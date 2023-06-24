namespace Whale.Objects.Network
{

    public class NetworkDTO : DockerObjectDTO
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Created { get; set; }
        public string? Scope { get; set; }
        public string? Driver { get; set; }
        public bool? EnableIPv6 { get; set; }
        public IPAMDTO? IPAM { get; set; }
        public bool? Internal { get; set; }
        public bool? Attachable { get; set; }
        public bool? Ingress { get; set; }
        public ConfigFromDTO? ConfigFrom { get; set; }
        public bool? ConfigOnly { get; set; }
        public ContainersDTO? Containers { get; set; }
        public OptionsDTO? Options { get; set; }
        public LabelsDTO? Labels { get; set; }
    }
}
