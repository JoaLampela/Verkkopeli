using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class GameApp
{
    private readonly ISceneFlowController _sceneFlowController;
    private readonly IAuthenticationService _authenticationService;
    private readonly StartupScenes _startupScenes;
    private bool _hasStarted;

    public GameApp(ISceneFlowController sfc, IAuthenticationService auth, StartupScenes scenes)
    {
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        _authenticationService = auth ?? throw new ArgumentNullException(nameof(auth));
        _startupScenes = scenes;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_hasStarted) return;

        _hasStarted = true;
        bool isSessionStored = await _authenticationService.TryRestoreSessionAsync(ct);
        ct.ThrowIfCancellationRequested();

        ScenePathSO startScene = isSessionStored
            ? _startupScenes.MainMenuSceneSO
            : _startupScenes.LoginSceneSO;

        _sceneFlowController.ChangePrimaryScene(startScene);
    }
}
