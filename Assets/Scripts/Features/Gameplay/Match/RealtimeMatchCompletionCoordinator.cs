using System;

public sealed class RealtimeMatchCompletionCoordinator : IDisposable
{
    private readonly MatchId _matchId;
    private readonly IMatchController _matchController;
    private readonly IMatchRealtimeEventSource _eventSource;

    public RealtimeMatchCompletionCoordinator(MatchId matchId, IMatchController mc, IMatchRealtimeEventSource mres)
    {
        _matchId = matchId;
        _matchController = mc ?? throw new ArgumentNullException(nameof(mc));
        _eventSource = mres ?? throw new ArgumentNullException(nameof(mres));
        _eventSource.MatchCompleted += HandleMatchCompletion;
    }

    public void Dispose()
    {
        _eventSource.MatchCompleted -= HandleMatchCompletion;
    }

    private void HandleMatchCompletion(MatchId matchId, PlayerId winnerId)
    {
        if (matchId != _matchId) return;
        
        _matchController.TryDeclareWinner(winnerId);
    }
}
