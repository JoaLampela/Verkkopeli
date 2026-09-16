using System;

public readonly struct AppDependencies
{
    public ISceneFlowController SceneFlowController { get; }
    public IPlayerProfileContext PlayerProfileContext { get; }
    public IGameplaySessionService GameplaySessionService { get; }
    public IGameplaySessionContextProvider GameplaySessionContextProvider => GameplaySessionService;
    public IMatchResultSink MatchResultSink { get; }
    public IAuthenticationService AuthenticationService { get; }
    public IMatchmakingService MatchmakingService { get; }
    public IRealtimeMessageSender RealtimeMessageSender { get; }
    public IMatchRealtimeEventSource MatchRealtimeEventSource { get; }
    public IMatchSessionService MatchSessionService { get; }

    public AppDependencies
    (
        ISceneFlowController sfc,
        IPlayerProfileContext ppc,
        IMatchResultSink mrs,
        IGameplaySessionService gss,
        IAuthenticationService auth,
        IMatchmakingService mms,
        IRealtimeMessageSender rtms,
        IMatchRealtimeEventSource mres,
        IMatchSessionService mss
    )
    {
        SceneFlowController = sfc ?? throw new ArgumentNullException(nameof(sfc));
        PlayerProfileContext = ppc ?? throw new ArgumentNullException(nameof(ppc));
        GameplaySessionService = gss ?? throw new ArgumentNullException(nameof(gss));
        MatchResultSink = mrs ?? throw new ArgumentNullException(nameof(mrs));
        AuthenticationService = auth ?? throw new ArgumentNullException(nameof(auth));
        MatchmakingService = mms ?? throw new ArgumentNullException(nameof(mms));
        RealtimeMessageSender = rtms ?? throw new ArgumentNullException(nameof(rtms));
        MatchRealtimeEventSource = mres ?? throw new ArgumentNullException(nameof(mres));
        MatchSessionService = mss ?? throw new ArgumentNullException(nameof(mss));
    }
}
