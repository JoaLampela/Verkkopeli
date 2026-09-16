public interface IPlayerProfileContext
{
    public bool HasProfile { get; }
    public PlayerProfile CurrentProfile { get; }
    public bool TryGetCurrentProfile(out PlayerProfile playerProfile);
}
