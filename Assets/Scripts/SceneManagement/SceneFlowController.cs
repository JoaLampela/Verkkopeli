using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneFlowController : MonoBehaviour, ISceneFlowController, ISceneTransitionEvents
{
    public event Action TransitionStarted;
    public event Action TransitionCompleted;
    public event Action<float> TransitionProgressChanged;
    private ISceneLoader _sceneLoader;
    private readonly List<Scene> _secondaryScenes = new();
    private AppDependencies? _sceneDependencies;
    private Scene _primaryScene;
    private bool _isTransitioning;

    public void Initialize(AppDependencies sceneDependencies)
    {
        _sceneDependencies = sceneDependencies;
    }

    public void Bind(ISceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
    }

    public void ChangePrimaryScene(ScenePathSO pathSO)
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        StartCoroutine(ChangePrimarySceneCoroutine(pathSO));
    }

    private IEnumerator ChangePrimarySceneCoroutine(ScenePathSO pathSO)
    {
        TransitionStarted?.Invoke();
        TransitionProgressChanged?.Invoke(0f);

        Scene previousPrimaryScene = _primaryScene;
        List<Scene> previousSecondaryScenes = _secondaryScenes.ToList();

        AsyncOperation loadingOperation = _sceneLoader.LoadAdditivelyAsync(pathSO);
        yield return TrackProgressCoroutine(loadingOperation, 0f, 0.8f);

        if (!_sceneLoader.TryGetLoadedScene(pathSO, out Scene scene))
        {
            _isTransitioning = false;
            TransitionCompleted?.Invoke(); // TODO: add TransitionFailed in the future
            Debug.LogError($"Could not obtain Scene with path: {pathSO.Path}");
            yield break;
        }

        _secondaryScenes.Clear();
        _sceneLoader.SetActive(scene);
        InitializeScene(scene);
        _primaryScene = scene;

        foreach (Scene secondaryScene in previousSecondaryScenes)
        {
            if (secondaryScene.IsValid() && secondaryScene.isLoaded)
                yield return _sceneLoader.UnloadAsync(secondaryScene);
        }

        if (previousPrimaryScene.IsValid() && previousPrimaryScene.isLoaded)
        {
            AsyncOperation unloadingOperation = _sceneLoader.UnloadAsync(previousPrimaryScene);
            yield return TrackProgressCoroutine(unloadingOperation, 0.8f, 1f);
        }

        TransitionProgressChanged?.Invoke(1f);
        TransitionCompleted?.Invoke();
        _isTransitioning = false;
    }

    private IEnumerator TrackProgressCoroutine(AsyncOperation operation, float start, float end)
    {
        while (!operation.isDone)
        {
            float operationProgress = Mathf.Clamp01(operation.progress);
            float transitionProgress = Mathf.Lerp(start, end, operationProgress);
            TransitionProgressChanged?.Invoke(transitionProgress);
            yield return null;
        }

        TransitionProgressChanged?.Invoke(end);
    }

    public void AddScene(ScenePathSO pathSO)
    {
        StartCoroutine(AddSceneCoroutine(pathSO));
    }

    private IEnumerator AddSceneCoroutine(ScenePathSO pathSO)
    {
        AsyncOperation loadingOperation = _sceneLoader.LoadAdditivelyAsync(pathSO);

        yield return loadingOperation;

        if (!_sceneLoader.TryGetLoadedScene(pathSO, out Scene scene)) yield break;

        InitializeScene(scene);
        _secondaryScenes.Add(scene);
    }

    public void RemoveScene(ScenePathSO pathSO)
    {
        if (!_sceneLoader.TryGetLoadedScene(pathSO, out Scene scene)) return;

        StartCoroutine(RemoveSceneCoroutine(scene));
    }

    public void RemoveScene(Scene scene)
    {
        StartCoroutine(RemoveSceneCoroutine(scene));
    }

    private IEnumerator RemoveSceneCoroutine(Scene scene)
    {
        if (scene.IsValid() && scene.isLoaded) yield return _sceneLoader.UnloadAsync(scene);

        _secondaryScenes.Remove(scene);
    }

    private void InitializeScene(Scene scene)
    {
        if (!_sceneDependencies.HasValue) throw new InvalidOperationException(nameof(InitializeScene));

        GameObject[] rootObjects = scene.GetRootGameObjects();

        if (rootObjects.Length != 1) throw new InvalidOperationException(nameof(InitializeScene));

        if (!rootObjects[0].TryGetComponent(out ILoadedSceneCompositionRoot root)) throw new InvalidOperationException(nameof(InitializeScene));

        root.Initialize(_sceneDependencies.Value);
    }
}
