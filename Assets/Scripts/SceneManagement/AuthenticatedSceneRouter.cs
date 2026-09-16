using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class AuthenticatedSceneRouter : IAuthenticatedSceneRouter
{
    private readonly ISceneFlowController _sceneFlowController;
    private readonly IMatchmakingService _matchmakingService;
    private readonly NavScenes _navScenes;

    public AuthenticatedSceneRouter(ISceneFlowController sfc, IMatchmakingService mms, NavScenes scenes)
    {
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        _matchmakingService = mms ?? throw new ArgumentNullException(nameof(mms));
        _navScenes = scenes;
    }

    public async Task RouteAsync(CancellationToken ct = default)
    {
        MatchInfo? activeMatch = await _matchmakingService.GetActiveMatchAsync(ct);
        ct.ThrowIfCancellationRequested();

        if (!activeMatch.HasValue)
        {
            _sceneFlowController.ChangePrimaryScene(_navScenes.MainMenuSceneSO);
            return;
        }

        ScenePathSO startScene = activeMatch.Value.MatchStatus switch
        {
            MatchStatus.Waiting => _navScenes.LobbySceneSO,
            MatchStatus.Running => _navScenes.GameplaySceneSO,
            _ => _navScenes.MainMenuSceneSO
        };
        _sceneFlowController.ChangePrimaryScene(startScene);
    }
}
