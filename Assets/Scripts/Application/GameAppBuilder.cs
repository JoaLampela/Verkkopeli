using System;

public sealed class GameAppBuilder
{
    private SceneFlowController _sceneFlowController;
    private LoadingScreenController _loadingScreenController;
    private NavScenes _startupScenes;
    private ISceneLoader _sceneLoader;
    private IMatchResultSink _matchResultSink;
    private IAuthenticationClient _authenticationClient;
    private IAuthenticationSessionStore _authSessionStore;
    private IPlayerProfileClient _profileClient;
    private IMatchClient _matchClient;
    private IMatchRealtimeConnection _matchConnection;
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

    public void RegisterStartupScenes(NavScenes scenes)
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

    public void RegisterMatchResultSink(IMatchResultSink mrs)
    {
        if (_matchResultSink != null) throw new InvalidOperationException(nameof(RegisterMatchResultSink));

        _matchResultSink = mrs ?? throw new ArgumentNullException(nameof(mrs));
    }

    public void RegisterAuthenticationClient(IAuthenticationClient auth)
    {
        if (_authenticationClient != null) throw new InvalidOperationException(nameof(RegisterAuthenticationClient));

        _authenticationClient = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    public void RegisterAuthenticationSessionStore(IAuthenticationSessionStore authSessionStore)
    {
        if (_authSessionStore != null) throw new InvalidOperationException(nameof(RegisterAuthenticationSessionStore));

        _authSessionStore = authSessionStore ?? throw new ArgumentNullException(nameof(authSessionStore));
    }

    public void RegisterPlayerProfileClient(IPlayerProfileClient profileClient)
    {
        if (_profileClient != null) throw new InvalidOperationException(nameof(RegisterPlayerProfileClient));

        _profileClient = profileClient ?? throw new ArgumentNullException(nameof(profileClient));
    }

    public void RegisterMatchClient(IMatchClient matchClient)
    {
        if (_matchClient != null) throw new InvalidOperationException(nameof(RegisterMatchClient));

        _matchClient = matchClient ?? throw new ArgumentNullException(nameof(matchClient));
    }

    public void RegisterMatchConnection(IMatchRealtimeConnection matchConnection)
    {
        if (_matchConnection != null) throw new InvalidOperationException(nameof(RegisterMatchConnection));

        _matchConnection = matchConnection ?? throw new ArgumentNullException(nameof(matchConnection));
    }

    public GameApp Build()
    {
        if (!HasValidRefs()) throw new InvalidOperationException(nameof(Build));

        GameplaySessionService gameplaySessionService = new();
        PlayerProfileService profileService = new();
        MatchSessionService matchSessionService = new();
        AuthenticationService authService = new(_authenticationClient, _profileClient, _authSessionStore, profileService);
        MatchmakingService matchmakingService = new(authService, matchSessionService, _matchClient, _matchConnection);
        AuthenticatedSceneRouter authSceneRouter = new(_sceneFlowController, matchmakingService, _startupScenes);
        _sceneFlowController.Bind(_sceneLoader);
        AppDependencies appDeps = new
            (
                _sceneFlowController,
                profileService,
                _matchResultSink,
                gameplaySessionService,
                authService,
                matchmakingService,
                _matchConnection,
                _matchConnection,
                matchSessionService,
                authSceneRouter
            );
        _sceneFlowController.Initialize(appDeps);
        _loadingScreenController.Initialize();
        _loadingScreenController.Bind(_sceneFlowController);
        return new GameApp(_sceneFlowController, authService, matchmakingService, authSceneRouter, _startupScenes);
    }

    private bool HasValidRefs()
    {
        return _sceneFlowController != null
            && _loadingScreenController != null
            && _startupScenes.LoginSceneSO != null
            && _startupScenes.MainMenuSceneSO != null
            && _sceneLoader != null
            && _matchResultSink != null
            && _authenticationClient != null
            && _profileClient != null
            && _authSessionStore != null
            && _matchClient != null
            && _matchConnection != null;
    }
}
