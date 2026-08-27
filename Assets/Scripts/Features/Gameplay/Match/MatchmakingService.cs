using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class MatchmakingService : IMatchmakingService
{
    private IAuthenticationContext _authContext;
    private IMatchClient _matchClient;
    private IMatchConnection _matchConnection;

    public MatchmakingService(IAuthenticationContext authContext, IMatchClient matchClient, IMatchConnection matchConnection)
    {
        _authContext = authContext ?? throw new ArgumentNullException(nameof(authContext));
        _matchClient = matchClient ?? throw new ArgumentNullException(nameof(matchClient));
        _matchConnection = matchConnection ?? throw new ArgumentNullException(nameof(matchConnection));
    }

    public async Task ConnectAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        await _matchConnection.ConnectAsync(matchId, accessToken, ct);
    }

    public async Task<MatchInfo> CreateMatchAsync(CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        return await _matchClient.CreateMatchAsync(accessToken, ct);
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        await _matchConnection.DisconnectAsync(ct);
    }

    public async Task<MatchInfo> JoinMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        return await _matchClient.JoinMatchAsync(matchId, accessToken, ct);
    }

    private AccessToken GetAccessToken()
    {
        if (!_authContext.TryGetAccessToken(out AccessToken accessToken))
            throw new InvalidOperationException(nameof(GetAccessToken));

        return accessToken;
    }
}
