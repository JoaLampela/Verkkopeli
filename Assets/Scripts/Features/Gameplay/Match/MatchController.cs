using System;

public sealed class MatchController : IMatchController
{
    public event Action<MatchResult> MatchCompleted;
    public bool IsCompleted => _isCompleted;
    private bool _isCompleted;
    private readonly MatchId _matchId;

    public MatchController(MatchId matchId)
    {
        if (matchId.Guid == Guid.Empty) throw new ArgumentException(nameof(matchId));

        _matchId = matchId;
    }

    public bool TryDeclareWinner(PlayerId playerId)
    {
        if (_isCompleted) return false;

        _isCompleted = true;
        MatchCompleted?.Invoke(new MatchResult(_matchId, playerId, DateTime.Now));
        return true;
    }
}
