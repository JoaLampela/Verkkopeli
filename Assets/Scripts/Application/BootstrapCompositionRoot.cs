using System;
using System.IO;
using UnityEngine;

public sealed class BootstrapCompositionRoot : MonoBehaviour
{
    [SerializeField] private SceneFlowController _sceneFlowController;
    [SerializeField] private LoadingScreenController _loadingScreenController;
    [SerializeField] private ScenePathSO _loginScenePathSO;
    [SerializeField] private ScenePathSO _mainMenuScenePathSO;
    private GameApp _app;

    private void Awake()
    {
        if (!HasRequiredReferences()) throw new InvalidOperationException("Missing dependencies!");

        string repoPath = Path.Combine(Application.persistentDataPath, AppConstants.Repository.ProfileDataLocation);
        string sinkPath = Path.Combine(Application.persistentDataPath, AppConstants.MatchHistory.MatchHistoryLocation);

        _app = new GameAppBuilder()
            .Add(new SceneLoader())
            .Add(new JsonProfileRepository(repoPath))
            .Add(new JsonMatchResultSink(sinkPath))
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
        return _sceneFlowController != null
        && _loadingScreenController != null
        && _loginScenePathSO != null
        && _mainMenuScenePathSO != null;
    }
}
