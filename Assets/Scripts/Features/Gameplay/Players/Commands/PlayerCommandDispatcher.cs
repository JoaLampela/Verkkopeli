using System;

public sealed class PlayerCommandDispatcher : IPlayerCommandDispatcher
{
    private readonly IPlayerRegistry _registry;

    public PlayerCommandDispatcher(IPlayerRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public bool Dispatch(PlayerId playerId, in PlayerCommand playerCommand)
    {
        if (!_registry.TryGet(playerId, out IPlayerActor playerActor)) return false;

        playerActor.Receive(playerCommand);
        return true;
    }
}
