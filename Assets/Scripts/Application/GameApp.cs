using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class GameApp
{
    private readonly ISceneFlowController _sceneFlowController;
    private readonly IPlayerProfileService _playerProfileService;
    private readonly StartupScenes _startupScenes;
    private bool _hasStarted;

    public GameApp(ISceneFlowController sfc, IPlayerProfileService pps, StartupScenes scenes)
    {
        _sceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        _playerProfileService = pps ?? throw new ArgumentNullException(nameof(pps));
        _startupScenes = scenes;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_hasStarted) return;

        _hasStarted = true;
        await _playerProfileService.InitializeAsync(ct);
        ct.ThrowIfCancellationRequested();

        ScenePathSO scene = _playerProfileService.HasProfile
        ? _startupScenes.MainMenuSceneSO
        : _startupScenes.LoginSceneSO;
        
        _sceneFlowController.ChangePrimaryScene(scene);
    }
}
