using System;
using UnityEngine;

public sealed class GoalTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    private IMatchController _matchController;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _playerLayer.value) == 0) return;

        PlayerActor playerActor = other.GetComponentInParent<PlayerActor>();

        if (playerActor == null) return;

        _matchController.TryDeclareWinner(playerActor.PlayerId);
    }

    public void Bind(IMatchController matchController)
    {
        _matchController = matchController ?? throw new ArgumentNullException(nameof(matchController));
    }
}
