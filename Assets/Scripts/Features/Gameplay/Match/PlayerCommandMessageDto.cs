using System;

[Serializable]
public sealed class PlayerCommandMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type = RealtimeProtocol.MessageTypes.PlayerCommand;
    public uint sequence;
    public float moveX;
    public float moveY;
    public float yawDegrees;
    public float pitchDegrees;
    public bool jumpPressed;
}
