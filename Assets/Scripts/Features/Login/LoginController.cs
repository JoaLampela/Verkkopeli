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
    private IAuthenticationService _authService;
    private IAuthenticatedSceneRouter _authSceneRouter;

    public void Bind(ISceneFlowController sceneFlow, IAuthenticationService authService, IAuthenticatedSceneRouter authSceneRouter)
    {
        _sceneFlowController = sceneFlow ?? throw new ArgumentNullException(nameof(sceneFlow));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _authSceneRouter = authSceneRouter ?? throw new ArgumentNullException(nameof(authSceneRouter));
    }

    public async void Login()
    {
        if (_continuationSceneSO == null
        || _emailInputField == null
        || _passwordInputField == null
        || _statusText == null)
            throw new InvalidOperationException(nameof(Login));

        try
        {
            _statusText.color = Color.white;
            _statusText.text = "Logging in...";
            await _authService.LoginAsync(_emailInputField.text, _passwordInputField.text, destroyCancellationToken);
            destroyCancellationToken.ThrowIfCancellationRequested();
            _statusText.color = Color.green;
            _statusText.text = "Logged in!";
            await _authSceneRouter.RouteAsync(destroyCancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            _statusText.color = Color.red;
            _statusText.text = "Invalid email or password.";
        }
        catch (OperationCanceledException) {} // Do nothing if operation is cancelled
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _statusText.color = Color.red;
            _statusText.text = "Unable to reach server.";
        }
    }
}
