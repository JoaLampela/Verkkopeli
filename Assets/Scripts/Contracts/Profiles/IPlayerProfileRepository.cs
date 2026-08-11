using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IPlayerProfileRepository
{
    public Task<IReadOnlyList<PlayerProfile>> LoadProfilesAsync(CancellationToken ct = default);
    public Task SaveProfilesAsync(IReadOnlyCollection<PlayerProfile> playerProfile, CancellationToken ct = default);
}
