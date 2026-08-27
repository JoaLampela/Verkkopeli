using System;
using System.IO;
using UnityEngine;

public sealed class BootstrapCompositionRoot : MonoBehaviour
{
    [SerializeField] private NetworkSettingsSO _settingsSO;
    [SerializeField] private SceneFlowController _sceneFlowController;
    [SerializeField] private LoadingScreenController _loadingScreenController;
    [SerializeField] private ScenePathSO _loginScenePathSO;
    [SerializeField] private ScenePathSO _mainMenuScenePathSO;
    private GameApp _app;

    private void Awake()
    {
        if (!HasRequiredReferences()) throw new InvalidOperationException("Missing dependencies!");

        string basePath = Application.persistentDataPath;
        string sinkPath = Path.Combine(basePath, AppConstants.MatchHistory.DataLocation);
        string authPath = Path.Combine(basePath, AppConstants.Authentication.DataLocation);
        NodeClient nodeClient = new(_settingsSO.ApiBaseUrl);
        NodeMatchConnection nodeMatchConnection = new(_settingsSO.RealtimeUri);
        IAuthenticationClient authClient = nodeClient;
        IPlayerProfileClient profileClient = nodeClient;
        IMatchClient matchClient = nodeClient;

        _app = new GameAppBuilder()
            .Add(new SceneLoader())
            .Add(new JsonMatchResultSink(sinkPath))
            .Add(new JsonAuthenticationSessionStore(authPath))
            .Add(authClient)
            .Add(profileClient)
            .Add(matchClient)
            .Add(nodeMatchConnection)
            .Add(new StartupScenes(_loginScenePathSO, _mainMenuScenePathSO))
            .Add(_sceneFlowController)
            .Add(_loadingScreenController)
            .Build();
    }

    private async Awaitable Start()
    {
        if (_app == null) throw new ArgumentNullException(nameof(_app));

        await _app.StartAsync(destroyCancellationToken);
    }

    private bool HasRequiredReferences()
    {
        return _settingsSO != null
        && _sceneFlowController != null
        && _loadingScreenController != null
        && _loginScenePathSO != null
        && _mainMenuScenePathSO != null;
    }
}
