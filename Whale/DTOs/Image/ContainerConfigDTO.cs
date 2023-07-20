namespace Whale.DTOs.Image
{
    public class ContainerConfigDTO
    {
        public string? Hostname { get; set; }
        public string? Domainname { get; set; }
        public string? User { get; set; }
        public bool AttachStdin { get; set; }
        public bool AttachStdout { get; set; }
        public bool AttachStderr { get; set; }
        public bool Tty { get; set; }
        public bool OpenStdin { get; set; }
        public bool StdinOnce { get; set; }
        public string[]? Env { get; set; }
        public string[]? Cmd { get; set; }
        public string? Image { get; set; }
        public object? Volumes { get; set; }
        public string? WorkingDir { get; set; }
        public object? Entrypoint { get; set; }
        public object? OnBuild { get; set; }
        public LabelsDTO? Labels { get; set; }
    }
}
