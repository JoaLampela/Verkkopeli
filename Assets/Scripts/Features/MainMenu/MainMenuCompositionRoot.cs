using System;
using System.Linq;
using UnityEngine;

public sealed class MainMenuCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private MainMenuController _mainMenuController;

    public void Initialize(AppDependencies dependencies)
    {
        if (dependencies.SceneFlowController is null) throw new ArgumentNullException(nameof(dependencies.SceneFlowController));

        GetComponentsInChildren<ISceneNavActions>(includeInactive: true).ToList().ForEach(action => action.Bind(dependencies.SceneFlowController, dependencies.AuthenticationService));

        if (_mainMenuController == null) throw new ArgumentNullException(nameof(_mainMenuController));

        _mainMenuController.Bind(dependencies.MatchmakingService, dependencies.SceneFlowController);
    }
}
