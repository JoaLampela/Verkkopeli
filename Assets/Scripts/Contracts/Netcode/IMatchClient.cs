using System.Threading;
using System.Threading.Tasks;

public interface IMatchClient
{
    public Task<MatchInfo> CreateMatchAsync(AccessToken accessToken, CancellationToken ct = default);
    public Task<MatchInfo> GetMatchAsync(MatchId matchId, CancellationToken ct = default);
    public Task<MatchInfo> JoinMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default);
    public Task LeaveMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default);
    public Task StartMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default);
    public Task CompleteMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default);
    public Task<MatchInfo?> GetActiveMatchAsync(AccessToken accessToken, CancellationToken ct = default);
}
