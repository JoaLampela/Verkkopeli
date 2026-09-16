using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class GameApp
{
    private readonly ISceneFlowController _sceneFlowController;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMatchmakingService _matchmakingService;
    private readonly IAuthenticatedSceneRouter _authSceneRouter;
    private readonly NavScenes _navScenes;
    private bool _hasStarted;

    public GameApp(ISceneFlowController sfc, IAuthenticationService auth, IMatchmakingService mms, IAuthenticatedSceneRouter asr, NavScenes scenes)
    {
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        _authenticationService = auth ?? throw new ArgumentNullException(nameof(auth));
        _matchmakingService = mms ?? throw new ArgumentNullException(nameof(mms));
        _authSceneRouter = asr ?? throw new ArgumentNullException(nameof(asr));
        _navScenes = scenes;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_hasStarted) return;

        _hasStarted = true;
        bool isAuthSessionStored = await _authenticationService.TryRestoreSessionAsync(ct);
        ct.ThrowIfCancellationRequested();

        if (!isAuthSessionStored)
        {
            _sceneFlowController.ChangePrimaryScene(_navScenes.LoginSceneSO);
            return;
        }

        await _authSceneRouter.RouteAsync(ct);
    }
}
