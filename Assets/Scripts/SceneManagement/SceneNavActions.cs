using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class SceneNavActions : MonoBehaviour, ISceneNavActions
{
    [SerializeField] private ScenePathSO _destinationPathSO;
    private ISceneFlowController _sceneFlowController;

    public void Bind(ISceneFlowController sceneFlowController)
    {
        _sceneFlowController = sceneFlowController ?? throw new ArgumentNullException(nameof(sceneFlowController));
    }

    public void ChangeToScene()
    {
        _sceneFlowController?.ChangePrimaryScene(_destinationPathSO);
    }

    public void AddScene()
    {
        _sceneFlowController?.AddScene(_destinationPathSO);
    }

    public void RemoveScene()
    {
        _sceneFlowController?.RemoveScene(_destinationPathSO);
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif

        Application.Quit();
    }
}
