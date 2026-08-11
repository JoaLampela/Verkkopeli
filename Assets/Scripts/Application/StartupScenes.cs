public readonly struct StartupScenes
{
    public ScenePathSO LoginSceneSO { get; }
    public ScenePathSO MainMenuSceneSO { get; }

    public StartupScenes(ScenePathSO loginSO, ScenePathSO menuSO)
    {
        LoginSceneSO = loginSO;
        MainMenuSceneSO = menuSO;
    }
}
