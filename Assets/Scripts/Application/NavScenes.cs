using System;

public readonly struct NavScenes
{
    public ScenePathSO LoginSceneSO { get; }
    public ScenePathSO MainMenuSceneSO { get; }
    public ScenePathSO LobbySceneSO { get; }
    public ScenePathSO GameplaySceneSO { get; }

    public NavScenes(ScenePathSO loginSO, ScenePathSO menuSO, ScenePathSO lobbySO, ScenePathSO gameplaySO)
    {
        LoginSceneSO = loginSO != null ? loginSO : throw new ArgumentNullException(nameof(loginSO));
        MainMenuSceneSO = menuSO != null ? menuSO : throw new ArgumentNullException(nameof(menuSO));
        LobbySceneSO = lobbySO != null ? lobbySO : throw new ArgumentNullException(nameof(lobbySO));
        GameplaySceneSO = gameplaySO != null ? gameplaySO : throw new ArgumentNullException(nameof(gameplaySO));
    }
}
