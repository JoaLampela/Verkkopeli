using System.Collections.Generic;

public interface IPlayerRegistry
{
    public IReadOnlyCollection<IPlayerActor> PlayerActors { get; }
    public bool Register(IPlayerActor playerActor);
    public bool TryUnregister(PlayerId playerId, out IPlayerActor playerActor);
    public bool TryGet(PlayerId playerId, out IPlayerActor playerActor);
}
