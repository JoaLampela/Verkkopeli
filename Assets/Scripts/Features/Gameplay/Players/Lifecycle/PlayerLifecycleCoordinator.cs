using System;
using UnityEngine;

public sealed class PlayerLifecycleCoordinator : IPlayerLifecycleCoordinator
{
    private readonly IPlayerSpawner _playerSpawner;
    private readonly SpawnPointSet _spawnPoints;

    public PlayerLifecycleCoordinator(IPlayerSpawner spawner, SpawnPointSet spawnPoints)
    {
        _playerSpawner = spawner ?? throw new ArgumentNullException(nameof(spawner));
        _spawnPoints = spawnPoints;
    }

    public IPlayerActor AddPlayer(PlayerProfile playerProfile)
    {
        int index = UnityEngine.Random.Range(0, _spawnPoints.Points.Count);
        Transform spawn = _spawnPoints.Points[index];
        return _playerSpawner.Spawn(playerProfile, spawn.position, spawn.rotation);
    }

    public bool RemovePlayer(PlayerId playerId)
    {
        return _playerSpawner.Despawn(playerId);
    }
}
