using UnityEngine;

public interface IPlayerMotor
{
    public void SetMovementIntent(Vector2 moveInput, float bodyYawDegrees);
    public void RequestJump();
}
