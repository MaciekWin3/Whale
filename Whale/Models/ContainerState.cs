namespace Whale.Models
{
    public enum ContainerState
    {
        Created,
        Running,
        Restarting,
        Exited,
        Paused,
        Removing,
        Dead,
        Unknown,
    }
}
