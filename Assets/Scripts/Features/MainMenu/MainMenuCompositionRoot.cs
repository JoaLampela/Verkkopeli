using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MainMenuCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    public void Initialize(AppDependencies dependencies)
    {
        if (dependencies.SceneFlowController is null) throw new ArgumentNullException();

        List<ISceneNavActions> navActions = GetComponentsInChildren<ISceneNavActions>(includeInactive: true).ToList();
        navActions.ForEach(action => action.Bind(dependencies.SceneFlowController));
    }
}
