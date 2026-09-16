using System;
using System.Collections.Generic;

[Serializable]
public sealed class MatchResponseDto
{
    public string matchId;
    public string hostPlayerId;
    public string status;
    public List<MatchParticipantDto> participants;
}
