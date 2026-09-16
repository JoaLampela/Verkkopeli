using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class SceneNavActions : MonoBehaviour, ISceneNavActions
{
    [SerializeField] private ScenePathSO _destinationPathSO;
    private ISceneFlowController _sceneFlowController;
    private IAuthenticationService _authService;

    public void Bind(ISceneFlowController sceneFlowController, IAuthenticationService authService)
    {
        _sceneFlowController = sceneFlowController ?? throw new ArgumentNullException(nameof(sceneFlowController));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async void ChangeToSceneAndLogOut()
    {
        try
        {
            await _authService.LogoutAsync(destroyCancellationToken);
            _sceneFlowController?.ChangePrimaryScene(_destinationPathSO);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
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
