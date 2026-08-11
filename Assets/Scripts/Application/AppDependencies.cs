using System;

public readonly struct AppDependencies
{
    public ISceneFlowController SceneFlowController { get; }
    public IPlayerProfileService PlayerProfileService { get; }
    public IMatchResultSink MatchResultSink { get; }
    public IPlayerProfileContext PlayerProfileContext => PlayerProfileService;
    public IGameplaySessionService GameplaySessionService { get; }
    public IGameplaySessionContextProvider GameplaySessionContextProvider => GameplaySessionService;

    public AppDependencies(ISceneFlowController sfc, IPlayerProfileService pps, IMatchResultSink mrs, IGameplaySessionService gss)
    {
        SceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        PlayerProfileService = pps ?? throw new ArgumentNullException(nameof(pps));
        MatchResultSink = mrs ?? throw new ArgumentNullException(nameof(mrs));
        GameplaySessionService = gss ?? throw new ArgumentNullException(nameof(gss));
    }
}
