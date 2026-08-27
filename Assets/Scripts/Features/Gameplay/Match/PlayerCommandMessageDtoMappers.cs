public static class PlayerCommandMessageDtoMappers
{
    public static PlayerCommandMessageDto ToDto(this PlayerCommand command)
    {
        return new PlayerCommandMessageDto
        {
            sequence = command.Sequence,
            moveX = command.MoveDir.x,
            moveY = command.MoveDir.y,
            yawDegrees = command.Look.YawDegrees,
            pitchDegrees = command.Look.PitchDegrees,
            jumpPressed = command.JumpPressed
        };
    }
}
