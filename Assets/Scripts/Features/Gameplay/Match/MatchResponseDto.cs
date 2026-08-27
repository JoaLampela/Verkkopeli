using System;
using System.Collections.Generic;

[Serializable]
public sealed class MatchResponseDto
{
    public string matchId;
    public string status;
    public List<MatchParticipantDto> participants;
}
