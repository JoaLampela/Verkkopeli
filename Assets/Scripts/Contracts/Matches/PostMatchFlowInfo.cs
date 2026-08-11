using System;

public readonly struct PostMatchFlowInfo
{
    public TimeSpan MinimumPostMatchTime { get; }

    public PostMatchFlowInfo(TimeSpan minTime)
    {
        MinimumPostMatchTime = minTime;
    }
}
