using System;

public sealed class LocalGameplaySessionContext : IGameplaySessionContext, IPostMatchFlowContext
{
    public IMatchResultSource MatchResultSource { get; }
    public IPostMatchFlowSource PostMatchFlowSource { get; }

    public LocalGameplaySessionContext(IMatchResultSource matchResultSource, IPostMatchFlowSource postMatchFlowSource)
    {
        MatchResultSource = matchResultSource ?? throw new ArgumentNullException(nameof(matchResultSource));
        PostMatchFlowSource = postMatchFlowSource ?? throw new ArgumentNullException(nameof(postMatchFlowSource));
    }
}
