using System;

public readonly struct MatchResult
{
    public MatchId MatchId { get; }
    public PlayerId WinnerId { get; }
    public DateTimeOffset EndTime { get; }

    public MatchResult(MatchId matchId, PlayerId winnerId, DateTimeOffset endTime)
    {
        MatchId = matchId;
        WinnerId = winnerId;
        EndTime = endTime;
    }
}
