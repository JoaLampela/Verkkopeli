using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameplayCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private PlayerActor _playerPrefab;
    [SerializeField] private GoalTrigger _goalTrigger;
    [SerializeField] private LocalPlayerInputSource _playerInputSource;
    [SerializeField] private LocalPlayerCommandProducer _playerCmdProd;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private ScenePathSO _gameplayUISceneSO;
    [SerializeField] private ScenePathSO _postMatchSceneSO;
    private IGameplaySessionService _gameplaySessionService;
    private IPostMatchFlowSource _matchCompletionCoordinator;

    private void OnDestroy()
    {
        _gameplaySessionService?.ClearCurrentSession();
        if (_matchCompletionCoordinator is IDisposable disposableCoordinator)
            disposableCoordinator?.Dispose();
    }

    public void Initialize(AppDependencies deps)
    {
        if (_gameplayUISceneSO == null)
            throw new ArgumentNullException(nameof(_gameplayUISceneSO));

        if (deps.SceneFlowController == null
        || deps.MatchResultSink == null
        || deps.GameplaySessionService == null)
            throw new ArgumentNullException(nameof(deps));

        if(!deps.PlayerProfileContext.TryGetCurrentProfile(out PlayerProfile profile))
            throw new InvalidOperationException(nameof(Initialize));

        BindSceneControlActions(deps);
        MatchId matchId = new(Guid.NewGuid());
        IMatchController matchController = new MatchController(matchId);
        _matchCompletionCoordinator = new LocalMatchCompletionCoordinator
        (
            matchController,
            deps.MatchResultSink,
            _playerInputSource,
            deps.SceneFlowController,
            _postMatchSceneSO,
            TimeSpan.FromSeconds(5f),
            destroyCancellationToken
        );
        LocalGameplaySessionContext gameplaySessionContext = new(matchController, _matchCompletionCoordinator);
        deps.GameplaySessionService.SetCurrentSession(gameplaySessionContext);
        _gameplaySessionService = deps.GameplaySessionService;
        _goalTrigger.Bind(matchController);
        IPlayerRegistry playerRegistry = new PlayerRegistry();
        IPlayerSpawner playerSpawner = new PlayerSpawner(_playerPrefab, playerRegistry);
        SpawnPointSet spawnPointSet = new(_spawnPoints);
        IPlayerLifecycleCoordinator lifecycleCoordinator = new PlayerLifecycleCoordinator(playerSpawner, spawnPointSet);
        IPlayerActor player = lifecycleCoordinator.AddPlayer(profile);
        IPlayerCommandDispatcher playerCmdDispatch = new PlayerCommandDispatcher(playerRegistry);
        IPlayerCommandSink playerCmdSink = new BoundLocalPlayerCommandSink(player.PlayerId, playerCmdDispatch);
        _playerCmdProd.Bind(_playerInputSource, playerCmdSink);
        _playerInputSource.Enable();
        deps.SceneFlowController.AddScene(_gameplayUISceneSO);
    }

    private void BindSceneControlActions(AppDependencies deps)
    {
        if (deps.SceneFlowController == null) throw new ArgumentNullException(nameof(deps));

        GetComponentsInChildren<ISceneNavActions>(includeInactive: true)
            .ToList()
            .ForEach(action => action.Bind(deps.SceneFlowController));
    }
}
