using System.Collections.Generic;

public readonly struct MatchInfo
{
    public MatchId MatchId { get; }
    public MatchStatus MatchStatus { get; }
    public IReadOnlyList<MatchParticipant> Participants { get; }

    public MatchInfo(MatchId matchId, MatchStatus status, IReadOnlyList<MatchParticipant> participants)
    {
        MatchId = matchId;
        MatchStatus = status;
        Participants = participants;
    }
}
