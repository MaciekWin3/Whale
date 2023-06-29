using Whale.Models.DockerVersion.DockerVersion;

namespace Whale.Models.Version
{
    public record DockerVersion
    {
        public Client Client { get; set; }
        public Server Server { get; set; }
    }
}
