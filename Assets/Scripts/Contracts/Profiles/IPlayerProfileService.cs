using System.Threading;
using System.Threading.Tasks;

public interface IPlayerProfileService : IPlayerProfileContext
{
    public Task InitializeAsync(CancellationToken ct = default);
    public Task<PlayerProfile> CreateProfileAsync(string displayName, PlayerColor playerColor, CancellationToken ct = default);
    public bool SelectProfile(PlayerId playerId);
    public Task SaveProfileAsync(PlayerProfile playerProfile, CancellationToken ct = default);
}
