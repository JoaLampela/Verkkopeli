using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets; // Will not work in WebGL. Weill need to use NativeWebSocket
using System;
using UnityEngine;
using System.Text;
using System.IO;

public sealed class NodeMatchConnection : IMatchRealtimeConnection
{
    public bool IsConnected => _socket?.State == WebSocketState.Open;
    private ClientWebSocket _socket;
    private readonly Uri _endpoint;
    private readonly SemaphoreSlim _sendGate = new(1, 1); // Sets threads using this resource to (init, max)

    public NodeMatchConnection(Uri endpoint)
    {
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    public async Task ConnectAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        if (IsConnected) throw new InvalidOperationException(nameof(ConnectAsync));

        if (_socket != null)
            await DisconnectAsync(ct);

        _socket = new ClientWebSocket();
        await _socket.ConnectAsync(_endpoint, ct);
        JoinMatchMessageDto joinDto = new()
        {
            matchId = matchId.ToString(),
            accessToken = accessToken.Value
        };
        await SendAsync(joinDto, ct);
        string responseJson = await ReceiveTextAsync(ct);
        MatchConnectedMessageDto connectedMsg = RealtimeProtocolParser.ParseMatchConnected(responseJson, matchId);
        Debug.Log($"Connected to realtime match: {connectedMsg.matchId}\nWith ID: {connectedMsg.playerId}");
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        if (_socket == null) return;

        await _sendGate.WaitAsync(ct);

        try
        {
            if (IsConnected || _socket.State == WebSocketState.CloseReceived)
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: "Disconnecting", ct);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _socket.Abort();
        }
        finally
        {
            _socket.Dispose();
            _socket = null;
            _sendGate.Release();
        }
    }

    private async Task<string> ReceiveTextAsync(CancellationToken ct = default)
    {
        if (!IsConnected)
            throw new InvalidOperationException(nameof(ReceiveTextAsync));

        byte[] buffer = new byte[4096];
        using MemoryStream stream = new();
        
        while (true)
        {
            WebSocketReceiveResult result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

            if (result.MessageType == WebSocketMessageType.Close)
                throw new InvalidOperationException("Connection closed mid-message");
            
            if (result.MessageType != WebSocketMessageType.Text)
                throw new InvalidOperationException("Message is not text");
            
            stream.Write(buffer, 0, result.Count);

            if (result.EndOfMessage)
                break;
        }
        
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public async Task SendAsync(IRealtimeMessage message, CancellationToken ct = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        string json = JsonUtility.ToJson(message);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        await _sendGate.WaitAsync(ct);
        
        try
        {
            ct.ThrowIfCancellationRequested();

            if (_socket == null || _socket.State != WebSocketState.Open) return;

            await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            _sendGate.Release();
        }
    }
}
