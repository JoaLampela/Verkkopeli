using System;

public sealed class GameAppBuilder
{
    private SceneFlowController _sceneFlowController;
    private LoadingScreenController _loadingScreenController;
    private StartupScenes _startupScenes;
    private ISceneLoader _sceneLoader;
    private IPlayerProfileRepository _repository;
    private IMatchResultSink _matchResultSink;
    private bool _startupScenesInitialized;

    public void RegisterSceneFlowController(SceneFlowController sfc)
    {
        if (_sceneFlowController != null) throw new InvalidOperationException(nameof(RegisterSceneFlowController));

        _sceneFlowController = sfc;
    }

    public void RegisterLoadingScreenController(LoadingScreenController lsc)
    {
        if (_loadingScreenController != null) throw new InvalidOperationException(nameof(RegisterLoadingScreenController));

        _loadingScreenController = lsc;
    }

    public void RegisterStartupScenes(StartupScenes scenes)
    {
        if (_startupScenesInitialized) throw new InvalidOperationException(nameof(RegisterStartupScenes));

        _startupScenesInitialized = true;
        _startupScenes = scenes;
    }

    public void RegisterSceneLoader(ISceneLoader sl)
    {
        if (_sceneLoader != null) throw new InvalidOperationException(nameof(RegisterSceneLoader));

        _sceneLoader = sl ?? throw new ArgumentNullException(nameof(sl));
    }

    public void RegisterPlayerProfileRepository(IPlayerProfileRepository repo)
    {
        if (_repository != null) throw new InvalidOperationException(nameof(RegisterPlayerProfileRepository));

        _repository = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public void RegisterMatchResultSink(IMatchResultSink mrs)
    {
        if (_matchResultSink != null) throw new InvalidOperationException(nameof(RegisterMatchResultSink));

        _matchResultSink = mrs ?? throw new ArgumentNullException(nameof(mrs));
    }

    public GameApp Build()
    {
        if (!HasValidRefs()) throw new InvalidOperationException(nameof(Build));

        PlayerProfileService profileService = new(_repository);
        GameplaySessionService gameplaySessionService = new();

        _sceneFlowController.Bind(_sceneLoader);
        AppDependencies sceneDeps = new
            (
                _sceneFlowController,
                profileService,
                _matchResultSink,
                gameplaySessionService
            );
        _sceneFlowController.Initialize(sceneDeps);
        _loadingScreenController.Initialize();
        _loadingScreenController.Bind(_sceneFlowController);

        return new GameApp(_sceneFlowController, profileService, _startupScenes);
    }

    private bool HasValidRefs()
    {
        return _sceneFlowController != null
            && _loadingScreenController != null
            && _startupScenes.LoginSceneSO != null
            && _startupScenes.MainMenuSceneSO != null
            && _sceneLoader != null
            && _repository != null
            && _matchResultSink != null;
    }
}
