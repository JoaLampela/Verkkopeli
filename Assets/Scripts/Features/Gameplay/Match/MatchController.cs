using System;

public sealed class MatchController : IMatchController
{
    public event Action<MatchResult> MatchCompleted;
    public bool IsCompleted => _isCompleted;
    private bool _isCompleted;
    private readonly MatchId _matchId;

    public MatchController(MatchId matchId)
    {
        _matchId = matchId.Guid != Guid.Empty
            ? matchId
            : throw new ArgumentException(nameof(matchId));
    }

    public bool TryDeclareWinner(PlayerId playerId)
    {
        if (_isCompleted) return false;

        _isCompleted = true;
        MatchCompleted?.Invoke(new MatchResult(_matchId, playerId, DateTimeOffset.UtcNow));
        return true;
    }
}
