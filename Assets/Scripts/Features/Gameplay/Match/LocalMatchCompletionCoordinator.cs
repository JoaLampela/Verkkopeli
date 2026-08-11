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
            ScenePathSO sceneSO,
            TimeSpan returnDelay,
            CancellationToken ct = default
        )
    {
        _matchResultSource = matchResultSource ?? throw new ArgumentNullException(nameof(matchResultSource));
        _matchResultSink = matchResultSink ?? throw new ArgumentNullException(nameof(matchResultSink));
        _playerInputSource = playerInputSource ?? throw new ArgumentNullException(nameof(playerInputSource));
        _sceneFlowController = sceneFlowController ?? throw new ArgumentNullException(nameof(sceneFlowController));
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
        _sceneFlowController.ChangePrimaryScene(_sceneSO);
    }
}
