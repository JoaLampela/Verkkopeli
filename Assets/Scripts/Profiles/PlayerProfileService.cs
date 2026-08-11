using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class PlayerProfileService : IPlayerProfileService
{
    public event Action<PlayerProfile> ProfileChanged;
    public bool HasProfile => _currentProfile != null;
    public PlayerProfile CurrentProfile => _currentProfile ?? throw new InvalidOperationException(nameof(CurrentProfile));
    private PlayerProfile? _currentProfile;
    private readonly IPlayerProfileRepository _repository;
    private readonly List<PlayerProfile> _profiles = new();

    public PlayerProfileService(IPlayerProfileRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        _profiles.Clear();
        IReadOnlyCollection<PlayerProfile> profiles = await _repository.LoadProfilesAsync(ct);
        _profiles.AddRange(profiles);

        if (profiles.Count == 0) return;

        SelectProfile(_profiles.First().PlayerId);
    }

    public async Task SaveProfileAsync(PlayerProfile profile, CancellationToken ct = default)
    {
        int index = _profiles.FindIndex(prof => prof.PlayerId == profile.PlayerId);

        if (index < 0) throw new InvalidOperationException(nameof(SaveProfileAsync));

        List<PlayerProfile> updatedProfiles = new(_profiles) { [index] = profile };
        await _repository.SaveProfilesAsync(updatedProfiles, ct);
        _profiles[index] = profile;

        if (!_currentProfile.HasValue || _currentProfile.Value.PlayerId != profile.PlayerId) return;

        _currentProfile = profile;
        ProfileChanged?.Invoke(profile);
    }

    public async Task<PlayerProfile> CreateProfileAsync(string displayName, PlayerColor playerColor, CancellationToken ct = default)
    {
        PlayerProfile profile = new(new(Guid.NewGuid()), displayName, playerColor);
        List<PlayerProfile> updatedProfiles = new(_profiles) { profile };
        await _repository.SaveProfilesAsync(updatedProfiles, ct);
        _profiles.Add(profile);

        if (_profiles.Count == 1) SelectProfile(profile.PlayerId);

        return profile;
    }

    public bool SelectProfile(PlayerId playerId)
    {
        if (!_profiles.Exists(prof => prof.PlayerId == playerId)) return false;

        PlayerProfile profile = _profiles.First(prof => prof.PlayerId == playerId);
        _currentProfile = profile;
        ProfileChanged?.Invoke(profile);
        return true;
    }

    public bool TryGetCurrentProfile(out PlayerProfile profile)
    {
        if (!HasProfile)
        {
            profile = default;
            return false;
        }

        profile = CurrentProfile;
        return true;
    }
}
