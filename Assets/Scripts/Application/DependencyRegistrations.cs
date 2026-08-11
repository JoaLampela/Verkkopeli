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

    public static GameAppBuilder Add(this GameAppBuilder builder, StartupScenes startupScenes)
    {
        builder.RegisterStartupScenes(startupScenes);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, ISceneLoader sceneLoader)
    {
        builder.RegisterSceneLoader(sceneLoader);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IPlayerProfileRepository playerProfileRepository)
    {
        builder.RegisterPlayerProfileRepository(playerProfileRepository);
        return builder;
    }

    public static GameAppBuilder Add(this GameAppBuilder builder, IMatchResultSink matchResultSink)
    {
        builder.RegisterMatchResultSink(matchResultSink);
        return builder;
    }
}
