using System;
using System.Collections.Generic;

[Serializable]
public sealed class MatchResultHistoryDto
{
    public string version;
    public List<MatchResultDto> results;
}
