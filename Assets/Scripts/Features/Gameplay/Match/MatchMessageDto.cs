using System;

[Serializable]
public sealed class MatchMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type;
    public string matchId;
}
