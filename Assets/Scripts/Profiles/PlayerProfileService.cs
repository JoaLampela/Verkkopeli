using System;

public sealed class PlayerProfileService : IPlayerProfileContext, IPlayerProfileSession
{
    public bool HasProfile => _currentProfile.HasValue;
    public PlayerProfile CurrentProfile => _currentProfile ?? throw new InvalidOperationException(nameof(CurrentProfile));
    private PlayerProfile? _currentProfile;

    public void ClearCurrentProfile()
    {
        _currentProfile = null;
    }

    public void SetCurrentProfile(PlayerProfile profile)
    {
        _currentProfile = profile;
    }

    public bool TryGetCurrentProfile(out PlayerProfile playerProfile)
    {
        if (_currentProfile.HasValue)
        {
            playerProfile = _currentProfile.Value;
            return true;
        }

        playerProfile = default;
        return false;
    }
}
