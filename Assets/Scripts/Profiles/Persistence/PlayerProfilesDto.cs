using System;
using System.Collections.Generic;

[Serializable]
public sealed class PlayerProfilesDto
{
    public string version;
    public List<PlayerProfileDto> profiles;
}
