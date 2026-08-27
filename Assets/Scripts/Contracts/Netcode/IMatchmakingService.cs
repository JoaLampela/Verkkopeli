using System.Threading;
using System.Threading.Tasks;

public interface IMatchmakingService
{
    public Task ConnectAsync(MatchId matchId, CancellationToken ct = default);
    public Task DisconnectAsync(CancellationToken ct = default);
    public Task<MatchInfo> CreateMatchAsync(CancellationToken ct = default);
    public Task<MatchInfo> JoinMatchAsync(MatchId matchId, CancellationToken ct = default);
}
