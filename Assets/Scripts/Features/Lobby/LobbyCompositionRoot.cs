using System;
using UnityEngine;

public sealed class LobbyCompositionRoot : MonoBehaviour, ILoadedSceneCompositionRoot
{
    [SerializeField] private LobbyController lobbyController;

    public void Initialize(AppDependencies dependencies)
    {
        if (lobbyController == null)
            throw new ArgumentNullException(nameof(lobbyController));

        if (dependencies.SceneFlowController == null)
            throw new InvalidOperationException(nameof(Initialize));

        lobbyController.Bind
        (
            dependencies.SceneFlowController,
            dependencies.MatchmakingService,
            dependencies.MatchSessionService,
            dependencies.MatchSessionService,
            dependencies.MatchRealtimeEventSource,
            dependencies.PlayerProfileContext
        );
    }
}
