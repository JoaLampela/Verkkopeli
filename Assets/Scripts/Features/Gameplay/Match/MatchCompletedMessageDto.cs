using System;

[Serializable]
public sealed class MatchCompletedMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type = RealtimeProtocol.MessageTypes.MatchCompleted;
    public string matchId;
    public string winnerPlayerId;
}
