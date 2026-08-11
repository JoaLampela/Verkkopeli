using System;
using System.Collections.Generic;
using System.Linq;

public static class PlayerProfileDtoMappers
{
    public static PlayerProfileDto ToDto(this PlayerProfile prof)
    {
        return new PlayerProfileDto
        {
            playerId = prof.PlayerId.Guid.ToString(),
            displayName = prof.DisplayName,
            red = prof.PlayerColor.Red,
            green = prof.PlayerColor.Green,
            blue = prof.PlayerColor.Blue
        };
    }

    public static PlayerProfile FromDto(this PlayerProfileDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.displayName)) throw new FormatException(nameof(dto.displayName));

        if (!Guid.TryParse(dto.playerId, out Guid playerGuid)) throw new FormatException(nameof(dto.playerId));

        return new PlayerProfile(new PlayerId(playerGuid), dto.displayName, new PlayerColor(dto.red, dto.green, dto.blue));
    }

    public static PlayerProfilesDto ToDto(this IEnumerable<PlayerProfile> profs)
    {
        if (profs == null) throw new ArgumentNullException(nameof(profs));

        return new PlayerProfilesDto
        {
            version = AppConstants.Repository.FormatVersion,
            profiles = profs.Select(prof => prof.ToDto()).ToList()
        };
    }

    public static List<PlayerProfile> FromDto(this PlayerProfilesDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        if (dto.profiles == null) throw new ArgumentNullException(nameof(dto.profiles));

        if (dto.version != AppConstants.Repository.FormatVersion) throw new NotSupportedException(dto.version);

        return dto.profiles.Select(prof => prof.FromDto()).ToList();
    }
}
