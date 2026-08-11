using UnityEngine;

public interface IPlayerActor : IPlayerCommandReceiver
{
    public GameObject RootObject { get; }
    public void Initialize(PlayerProfile playerProfile);
}
