using System;

public sealed class MatchSessionService : IMatchSessionService
{
    public bool HasMatch => _match != null;
    public MatchInfo CurrentMatch => _match ?? throw new InvalidOperationException(nameof(CurrentMatch));
    private MatchInfo? _match;

    public void ClearCurrentMatch()
    {
        _match = null;
    }

    public void SetCurrentMatch(MatchInfo matchInfo)
    {
        _match = matchInfo;
    }

    public bool TryGetCurrentMatch(out MatchInfo matchInfo)
    {
        if (!_match.HasValue)
        {
            matchInfo = default;
            return false;
        }

        matchInfo = _match.Value;
        return true;
    }
}
