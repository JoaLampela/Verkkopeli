using System;

[Serializable]
public sealed class JoinMatchMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type = RealtimeProtocol.MessageTypes.JoinMatch;
    public string matchId;
    public string accessToken;
}
