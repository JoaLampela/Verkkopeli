using System;

[Serializable]
public sealed class MatchConnectedMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type = RealtimeProtocol.MessageTypes.MatchConnected;
    public string matchId;
    public string playerId;
}
