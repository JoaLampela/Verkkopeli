using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerProfilePresenter : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _visuals;

    public void Apply(PlayerProfile profile)
    {
        Color32 playerColor = new(profile.PlayerColor.Red, profile.PlayerColor.Green, profile.PlayerColor.Blue, byte.MaxValue);
        _visuals.ForEach(visual => visual.material.color = playerColor);
    }
}
