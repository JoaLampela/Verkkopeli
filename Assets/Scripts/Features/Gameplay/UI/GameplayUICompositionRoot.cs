using System;
using UnityEngine;

public sealed class GameplayUICompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private GameplayUIController _gameplayUIController;

    public void Initialize(AppDependencies deps)
    {
        if (_gameplayUIController == null) throw new ArgumentNullException(nameof(_gameplayUIController));

        IPlayerProfileContext playerProfileContext = deps.PlayerProfileContext ?? throw new ArgumentNullException(nameof(deps));

        if (!playerProfileContext.HasProfile) throw new ArgumentException(nameof(deps));

        if (!deps.GameplaySessionContextProvider.TryGetSessionContext(out IGameplaySessionContext gameplaySessionContext))
            throw new InvalidOperationException(nameof(Initialize));

        if (gameplaySessionContext is not IPostMatchFlowContext postMatchFlowContext)
            throw new InvalidOperationException(nameof(Initialize));
        
        _gameplayUIController.Bind(deps.PlayerProfileContext, gameplaySessionContext.MatchResultSource, postMatchFlowContext);
    }
}
