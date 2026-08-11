using System;
using UnityEngine;

public sealed class BoundLocalPlayerCommandSink : IPlayerCommandSink
{
    private readonly IPlayerCommandDispatcher _playerCmdDispatch;
    private readonly PlayerId _playerId;

    public BoundLocalPlayerCommandSink(PlayerId playerId, IPlayerCommandDispatcher commandDispatcher)
    {
        _playerId = playerId;
        _playerCmdDispatch = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
    }

    public void Submit(in PlayerCommand playerCommand)
    {
        if (!_playerCmdDispatch.Dispatch(_playerId, playerCommand))
            Debug.LogWarning($"No PlayerActor was registered for {_playerId}.");
    }
}
