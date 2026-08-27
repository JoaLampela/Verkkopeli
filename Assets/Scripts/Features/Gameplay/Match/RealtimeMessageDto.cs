using System;

[Serializable]
public sealed class RealtimeMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type;
}
