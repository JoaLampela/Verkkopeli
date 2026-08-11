using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    public AsyncOperation LoadAdditivelyAsync(ScenePathSO pathSO);
    public AsyncOperation UnloadAsync(Scene scene);
    public void SetActive(Scene scene);
    public bool TryGetLoadedScene(ScenePathSO pathSO, out Scene scene);
}
