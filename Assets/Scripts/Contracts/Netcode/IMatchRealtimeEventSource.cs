using System;

public interface IMatchRealtimeEventSource
{
    public event Action<MatchId> MatchStarted;
    public event Action<MatchId> MatchCancelled;
    public event Action<MatchId, PlayerId> MatchCompleted;
    public event Action<MatchId> ParticipantJoined;
    public event Action<MatchId> ParticipantLeft;
}
