using System;

[Serializable]
public sealed class ErrorMessageDto : IRealtimeMessage
{
    public string MessageType => type;
    public string type = RealtimeProtocol.MessageTypes.Error;
    public string code;
    public string message;
}
