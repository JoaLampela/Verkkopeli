using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MainMenuCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private MainMenuController _mainMenuController;

    public void Initialize(AppDependencies dependencies)
    {
        if (dependencies.SceneFlowController is null) throw new ArgumentNullException(nameof(dependencies.SceneFlowController));

        List<ISceneNavActions> navActions = GetComponentsInChildren<ISceneNavActions>(includeInactive: true).ToList();
        navActions.ForEach(action => action.Bind(dependencies.SceneFlowController));

        if (_mainMenuController == null) throw new ArgumentNullException(nameof(_mainMenuController));

        _mainMenuController.Bind(dependencies.MatchmakingService, dependencies.SceneFlowController);
    }
}
