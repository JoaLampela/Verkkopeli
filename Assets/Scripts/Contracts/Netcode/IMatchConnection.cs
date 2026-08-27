using System.Threading;
using System.Threading.Tasks;

public interface IMatchConnection
{
    public bool IsConnected { get; }
    public Task ConnectAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default);
    public Task DisconnectAsync(CancellationToken ct = default);
}
