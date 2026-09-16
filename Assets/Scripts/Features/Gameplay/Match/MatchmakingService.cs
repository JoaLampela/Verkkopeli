using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class MatchmakingService : IMatchmakingService
{
    private readonly IAuthenticationContext _authContext;
    private readonly IMatchSessionService _matchSessionService;
    private readonly IMatchClient _matchClient;
    private readonly IMatchConnection _matchConnection;

    public MatchmakingService(IAuthenticationContext authContext, IMatchSessionService matchSessionService, IMatchClient matchClient, IMatchConnection matchConnection)
    {
        _authContext = authContext ?? throw new ArgumentNullException(nameof(authContext));
        _matchSessionService = matchSessionService ?? throw new ArgumentNullException(nameof(matchSessionService));
        _matchClient = matchClient ?? throw new ArgumentNullException(nameof(matchClient));
        _matchConnection = matchConnection ?? throw new ArgumentNullException(nameof(matchConnection));
    }

    public async Task CompleteMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        await _matchClient.CompleteMatchAsync(matchId, accessToken, ct);
    }

    public async Task ConnectAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        await _matchConnection.ConnectAsync(matchId, accessToken, ct);
    }

    public async Task<MatchInfo> CreateMatchAsync(CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        MatchInfo matchInfo = await _matchClient.CreateMatchAsync(accessToken, ct);
        _matchSessionService.SetCurrentMatch(matchInfo);
        return matchInfo;
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        await _matchConnection.DisconnectAsync(ct);
    }

    public async Task<MatchInfo?> GetActiveMatchAsync(CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        MatchInfo? matchInfo = await _matchClient.GetActiveMatchAsync(accessToken, ct);

        if (!matchInfo.HasValue)
        {
            _matchSessionService.ClearCurrentMatch();
            return null;
        }
        
        _matchSessionService.SetCurrentMatch(matchInfo.Value);
        await _matchConnection.ConnectAsync(matchInfo.Value.MatchId, accessToken, ct);
        return matchInfo;
    }

    public async Task<MatchInfo> JoinMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        MatchInfo matchInfo = await _matchClient.JoinMatchAsync(matchId, accessToken, ct);
        _matchSessionService.SetCurrentMatch(matchInfo);
        return matchInfo;
    }

    public async Task LeaveMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        await _matchClient.LeaveMatchAsync(matchId, accessToken, ct);
        _matchSessionService.ClearCurrentMatch();
    }

    public async Task<MatchInfo> RefreshCurrentMatchAsync(CancellationToken ct = default)
    {
        if (!_matchSessionService.TryGetCurrentMatch(out MatchInfo matchInfo))
            throw new InvalidOperationException("No active current match");

        MatchInfo updatedMatchInfo = await _matchClient.GetMatchAsync(matchInfo.MatchId, ct);
        _matchSessionService.SetCurrentMatch(updatedMatchInfo);
        return updatedMatchInfo;
    }

    public async Task StartMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        AccessToken accessToken = GetAccessToken();
        await _matchClient.StartMatchAsync(matchId, accessToken, ct);
    }

    private AccessToken GetAccessToken()
    {
        if (!_authContext.TryGetAccessToken(out AccessToken accessToken))
            throw new InvalidOperationException(nameof(GetAccessToken));

        return accessToken;
    }
}
