using UnityEngine;

public readonly struct PlayerInputFrame
{
    public Vector2 MoveDir { get; }
    public Vector2 LookDelta { get; }
    public bool JumpPressed { get; }

    public PlayerInputFrame(Vector2 moveDir, Vector2 lookDelta, bool jumpPressed)
    {
        MoveDir = moveDir;
        LookDelta = lookDelta;
        JumpPressed = jumpPressed;
    }
}
