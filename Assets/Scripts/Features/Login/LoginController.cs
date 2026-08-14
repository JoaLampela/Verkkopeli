using System;
using TMPro;
using UnityEngine;

public sealed class LoginController : MonoBehaviour
{
    [SerializeField] private ScenePathSO _continuationSceneSO;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_Text _statusText;
    private ISceneFlowController _sceneFlowController;
    private IAuthenticationService _authenticationService;

    public void Bind(ISceneFlowController sceneFlow, IAuthenticationService authService)
    {
        _sceneFlowController = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
        _authenticationService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async void Login()
    {
        if (_continuationSceneSO == null
        || _emailInputField == null
        || _passwordInputField == null
        || _statusText == null) throw new InvalidOperationException(nameof(Login));

        try
        {
            _statusText.color = Color.white;
            _statusText.text = "Logging in...";
            await _authenticationService.LoginAsync(_emailInputField.text, _passwordInputField.text, destroyCancellationToken);
            destroyCancellationToken.ThrowIfCancellationRequested();
            _statusText.color = Color.green;
            _statusText.text = "Logged in!";
            _sceneFlowController.ChangePrimaryScene(_continuationSceneSO);
        }
        catch (UnauthorizedAccessException)
        {
            _statusText.color = Color.red;
            _statusText.text = "Invalid email or password.";
        }
        catch (OperationCanceledException) {}
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _statusText.color = Color.red;
            _statusText.text = "Unable to reach server.";
        }
    }
}
