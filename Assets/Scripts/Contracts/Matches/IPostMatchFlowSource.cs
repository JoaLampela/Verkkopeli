using System;

public interface IPostMatchFlowSource
{
    public event Action<PostMatchFlowInfo> PostMatchStarted;
}
