public interface IMatchContext
{
    public bool HasMatch { get; }
    public MatchInfo CurrentMatch { get; }
    public bool TryGetCurrentMatch(out MatchInfo matchInfo);
}
