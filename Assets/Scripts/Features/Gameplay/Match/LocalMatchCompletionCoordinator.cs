using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class LocalMatchCompletionCoordinator : IPostMatchFlowSource, IDisposable
{
    public event Action<PostMatchFlowInfo> PostMatchStarted;
    private readonly IMatchResultSource _matchResultSource;
    private readonly IMatchResultSink _matchResultSink;
    private readonly IPlayerInputSource _playerInputSource;
    private readonly ISceneFlowController _sceneFlowController;
    private readonly IMatchmakingService _matchmakingService;
    private readonly IPlayerProfileContext _profileContext;
    private readonly IMatchSession _matchSession;
    private readonly ScenePathSO _sceneSO;
    private readonly TimeSpan _returnDelay;
    private readonly CancellationTokenSource _lifetimeCancellation;
    private bool _handlingCompletion;

    public LocalMatchCompletionCoordinator
        (
            IMatchResultSource matchResultSource,
            IMatchResultSink matchResultSink,
            IPlayerInputSource playerInputSource,
            ISceneFlowController sceneFlowController,
            IMatchmakingService matchmakingService,
            IPlayerProfileContext profileContext,
            IMatchSession matchSession,
            ScenePathSO sceneSO,
            TimeSpan returnDelay,
            CancellationToken ct = default
        )
    {
        _matchResultSource = matchResultSource ?? throw new ArgumentNullException(nameof(matchResultSource));
        _matchResultSink = matchResultSink ?? throw new ArgumentNullException(nameof(matchResultSink));
        _playerInputSource = playerInputSource ?? throw new ArgumentNullException(nameof(playerInputSource));
        _sceneFlowController = sceneFlowController ?? throw new ArgumentNullException(nameof(sceneFlowController));
        _matchmakingService = matchmakingService ?? throw new ArgumentNullException(nameof(matchmakingService));
        _profileContext = profileContext ?? throw new ArgumentNullException(nameof(profileContext));
        _matchSession = matchSession ?? throw new ArgumentNullException(nameof(matchSession));
        _sceneSO = sceneSO != null ? sceneSO : throw new ArgumentNullException(nameof(sceneSO));
        _returnDelay = returnDelay;
        _lifetimeCancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _matchResultSource.MatchCompleted += CompleteMatch;
    }

    public void Dispose()
    {
        _matchResultSource.MatchCompleted -= CompleteMatch;
        _lifetimeCancellation.Cancel();
        _lifetimeCancellation.Dispose();
    }

    private async void CompleteMatch(MatchResult matchResult)
    {
        if (_handlingCompletion) return;

        _handlingCompletion = true;
        PostMatchStarted?.Invoke(new PostMatchFlowInfo(_returnDelay));
        await HandleCompletionAsync(matchResult, _lifetimeCancellation.Token);
    }

    private async Task HandleCompletionAsync(MatchResult matchResult, CancellationToken ct = default)
    {
        Task delayTask = Task.Delay(_returnDelay, ct);

        try
        {
            await _matchResultSink.HandleResultAsync(matchResult, ct);

            if (!_profileContext.TryGetCurrentProfile(out PlayerProfile profile))
                throw new InvalidOperationException("No local PlayerProfile!");
            
            if (profile.PlayerId == matchResult.WinnerId)
                await _matchmakingService.CompleteMatchAsync(matchResult.MatchId, ct);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        try
        {
            await delayTask;
            ct.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            return;
        }

        _playerInputSource.Disable();
        await _matchmakingService.DisconnectAsync(ct);
        _matchSession.ClearCurrentMatch();
        _sceneFlowController.ChangePrimaryScene(_sceneSO);
    }
}
