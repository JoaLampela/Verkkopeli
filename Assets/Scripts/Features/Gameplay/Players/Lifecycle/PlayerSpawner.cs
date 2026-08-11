using System;
using UnityEngine;

public sealed class PlayerSpawner : IPlayerSpawner
{
    private readonly IPlayerRegistry _playerRegistry;
    private readonly PlayerActor _playerPrefab;

    public PlayerSpawner(PlayerActor prefab, IPlayerRegistry registry)
    {
        _playerPrefab = prefab != null
        ? prefab
        : throw new ArgumentNullException(nameof(prefab));

        _playerRegistry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public IPlayerActor Spawn(PlayerProfile playerProfile, Vector3 position, Quaternion rotation)
    {
        PlayerActor player = UnityEngine.Object.Instantiate(_playerPrefab, position, rotation);
        player.Initialize(playerProfile);

        if (!_playerRegistry.Register(player))
        {
            UnityEngine.Object.Destroy(player.RootObject);
            throw new InvalidOperationException(nameof(Spawn));
        }

        return player;
    }

    public bool Despawn(PlayerId playerId)
    {
        if (!_playerRegistry.TryUnregister(playerId, out IPlayerActor playerActor)) return false;

        UnityEngine.Object.Destroy(playerActor.RootObject);
        return true;
    }
}
