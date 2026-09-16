using System.Collections.Generic;

public readonly struct MatchInfo
{
    public MatchId MatchId { get; }
    public PlayerId HostPlayerId { get; }
    public MatchStatus MatchStatus { get; }
    public IReadOnlyList<MatchParticipant> Participants { get; }

    public MatchInfo(MatchId matchId, PlayerId hostPlayerId, MatchStatus status, IReadOnlyList<MatchParticipant> participants)
    {
        MatchId = matchId;
        HostPlayerId = hostPlayerId;
        MatchStatus = status;
        Participants = participants;
    }
}
