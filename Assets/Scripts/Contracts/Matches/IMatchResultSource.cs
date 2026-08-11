using System;

public interface IMatchResultSource
{
    public event Action<MatchResult> MatchCompleted;
    public bool IsCompleted { get; }
}
