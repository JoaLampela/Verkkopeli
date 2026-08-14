using System;

public interface IPlayerProfileContext
{
    public bool HasProfile { get; }
    PlayerProfile CurrentProfile { get; }
    public bool TryGetCurrentProfile(out PlayerProfile playerProfile);
}
