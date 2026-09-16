public static class DependencyRegistrations
{
    public static GameAppBuilder Add(this GameAppBuilder builder, SceneFlowController sceneFlowController)
    {
        builder.RegisterSceneFlowController(sceneFlowController);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, LoadingScreenController loadingScreenController)
    {
        builder.RegisterLoadingScreenController(loadingScreenController);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, NavScenes startupScenes)
    {
        builder.RegisterStartupScenes(startupScenes);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, ISceneLoader sceneLoader)
    {
        builder.RegisterSceneLoader(sceneLoader);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IMatchResultSink matchResultSink)
    {
        builder.RegisterMatchResultSink(matchResultSink);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IAuthenticationClient authClient)
    {
        builder.RegisterAuthenticationClient(authClient);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IAuthenticationSessionStore authSessionStore)
    {
        builder.RegisterAuthenticationSessionStore(authSessionStore);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IPlayerProfileClient profileClient)
    {
        builder.RegisterPlayerProfileClient(profileClient);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IMatchClient matchClient)
    {
        builder.RegisterMatchClient(matchClient);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IMatchRealtimeConnection matchConnection)
    {
        builder.RegisterMatchConnection(matchConnection);
        return builder;
    }
}
