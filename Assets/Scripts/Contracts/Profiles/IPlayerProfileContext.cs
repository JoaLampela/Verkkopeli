using System;

public interface IPlayerProfileContext
{
    public event Action<PlayerProfile> ProfileChanged;
    public bool HasProfile { get; }
    PlayerProfile CurrentProfile { get; }
    public bool TryGetCurrentProfile(out PlayerProfile playerProfile);
}
