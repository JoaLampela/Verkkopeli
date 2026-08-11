using UnityEngine;

public interface IPlayerSpawner
{
    public IPlayerActor Spawn(PlayerProfile playerProfile, Vector3 position, Quaternion rotation);
    public bool Despawn(PlayerId playerId);
}
