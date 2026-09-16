using System;
using UnityEngine;

public static class RealtimeProtocolParsers
{
    public static MatchConnectedMessageDto ParseMatchConnected(string json, MatchId expectedMatchId)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException(nameof(json));
        
        RealtimeMessageDto msg = JsonUtility.FromJson<RealtimeMessageDto>(json)
            ?? throw new ArgumentException(nameof(json));
        
        if (msg.MessageType == RealtimeProtocol.MessageTypes.Error)
        {
            ErrorMessageDto error = JsonUtility.FromJson<ErrorMessageDto>(json)
                ?? throw new ArgumentException(nameof(json));
            
            throw new InvalidOperationException($"Error {error.code}: {error.message}");
        }

        if (msg.MessageType != RealtimeProtocol.MessageTypes.MatchConnected)
            throw new InvalidOperationException($"Invalid message type: {msg.MessageType}");
        
        MatchConnectedMessageDto connectMsg = JsonUtility.FromJson<MatchConnectedMessageDto>(json)
            ?? throw new ArgumentException(nameof(json));
        
        if (connectMsg == null || connectMsg.matchId != expectedMatchId.ToString())
            throw new InvalidOperationException($"Invalid MatchId: {connectMsg.matchId}");
        
        return connectMsg;
    }
}
