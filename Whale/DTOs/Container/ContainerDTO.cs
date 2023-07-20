using Whale.DTOs;

namespace Whale.DTOs.Container
{
    public class ContainerDTO : DockerObjectDTO
    {
        public string? Id { get; set; }
        public string? Created { get; set; }
        public string? Path { get; set; }
        public object[]? Args { get; set; }
        public StateDTO? State { get; set; }
        public string? Image { get; set; }
        public string? ResolvConfPath { get; set; }
        public string? HostnamePath { get; set; }
        public string? HostsPath { get; set; }
        public string? LogPath { get; set; }
        public string? Name { get; set; }
        public int RestartCount { get; set; }
        public string? Driver { get; set; }
        public string? Platform { get; set; }
        public string? MountLabel { get; set; }
        public string? ProcessLabel { get; set; }
        public string? AppArmorProfile { get; set; }
        public object? ExecIDs { get; set; }
        public HostConfigDTO? HostConfig { get; set; }
        public GraphDriverDTO? GraphDriver { get; set; }
        public object[]? Mounts { get; set; }
        public ConfigDTO? Config { get; set; }
        public NetworksettingsDTO? NetworkSettings { get; set; }
    }
}
