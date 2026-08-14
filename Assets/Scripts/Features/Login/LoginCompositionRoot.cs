using System;
using UnityEngine;

public sealed class LoginCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private LoginController _loginController;

    public void Initialize(AppDependencies dependencies)
    {
        if (_loginController == null) throw new ArgumentNullException(nameof(_loginController));

        _loginController.Bind(dependencies.SceneFlowController, dependencies.AuthenticationService);
    }
}
