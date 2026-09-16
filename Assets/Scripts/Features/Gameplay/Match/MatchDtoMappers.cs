using System;
using System.Collections.Generic;
using System.Linq;

public static class MatchDtoMappers
{
    public static MatchInfo FromDto(this MatchResponseDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (!Guid.TryParse(dto.matchId, out Guid matchGuid))
            throw new FormatException(nameof(dto.matchId));

        if (!Guid.TryParse(dto.hostPlayerId, out Guid hostPlayerGuid))
            throw new FormatException(nameof(dto.hostPlayerId));

        if (!Enum.TryParse(dto.status, ignoreCase: true, out MatchStatus status))
            throw new FormatException(nameof(dto.status));

        List<MatchParticipant> participants = dto.participants?.Select(part => part.FromDto()).ToList() ?? new List<MatchParticipant>();

        return new MatchInfo(new MatchId(matchGuid), new PlayerId(hostPlayerGuid), status, participants);
    }

    public static MatchParticipant FromDto(this MatchParticipantDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.displayName))
            throw new FormatException(nameof(dto.displayName));

        if (!Guid.TryParse(dto.playerId, out Guid guid))
            throw new FormatException(nameof(dto.playerId));

        return new MatchParticipant(new PlayerId(guid), dto.displayName);
    }
}
