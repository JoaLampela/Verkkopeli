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
    private IDisposable _realtimeCompletionCoordinator;

    private void OnDestroy()
    {
        _gameplaySessionService?.ClearCurrentSession();
        
        if (_matchCompletionCoordinator is IDisposable disposableCoordinator)
            disposableCoordinator?.Dispose();

        _realtimeCompletionCoordinator?.Dispose();
    }

    public void Initialize(AppDependencies deps)
    {
        if (_gameplayUISceneSO == null)
            throw new ArgumentNullException(nameof(_gameplayUISceneSO));

        if (deps.SceneFlowController == null
        || deps.MatchResultSink == null
        || deps.GameplaySessionService == null)
            throw new ArgumentNullException(nameof(deps));

        if (!deps.MatchSessionService.TryGetCurrentMatch(out MatchInfo matchInfo))
            throw new InvalidOperationException("No valid match when entering gameplay");

        if (!deps.PlayerProfileContext.TryGetCurrentProfile(out PlayerProfile profile))
            throw new InvalidOperationException(nameof(Initialize));

        BindSceneControlActions(deps);
        IMatchController matchController = new MatchController(matchInfo.MatchId);
        RealtimeMatchCompletionCoordinator realtimeCompletionCoordinator = new
        (
            matchInfo.MatchId,
            matchController,
            deps.MatchRealtimeEventSource
        );
        _realtimeCompletionCoordinator = realtimeCompletionCoordinator;
        _matchCompletionCoordinator = new LocalMatchCompletionCoordinator
        (
            matchController,
            deps.MatchResultSink,
            _playerInputSource,
            deps.SceneFlowController,
            deps.MatchmakingService,
            deps.PlayerProfileContext,
            deps.MatchSessionService,
            _postMatchSceneSO,
            TimeSpan.FromSeconds(5d),
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
        IPlayerCommandSink localCmdSink = new BoundLocalPlayerCommandSink(player.PlayerId, playerCmdDispatch);
        IPlayerCommandSink networkCmdSink = new NetworkPlayerCommandSink(deps.RealtimeMessageSender, destroyCancellationToken);
        IPlayerCommandSink playerCmdSink = new CompositePlayerCommandSink(localCmdSink, networkCmdSink);
        _playerCmdProd.Bind(_playerInputSource, playerCmdSink);
        _playerInputSource.Enable();
        deps.SceneFlowController.AddScene(_gameplayUISceneSO);
    }

    private void BindSceneControlActions(AppDependencies deps)
    {
        if (deps.SceneFlowController == null)
            throw new ArgumentNullException(nameof(deps));

        GetComponentsInChildren<ISceneNavActions>(includeInactive: true).ToList().ForEach(action => action.Bind(deps.SceneFlowController, deps.AuthenticationService));
    }
}
