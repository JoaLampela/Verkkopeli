using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets; // Will not work in WebGL. Will need to use NativeWebSocket for that
using System;
using UnityEngine;
using System.Text;
using System.IO;

public sealed class NodeMatchConnection : IMatchRealtimeConnection
{
    public event Action<MatchId> MatchStarted;
    public event Action<MatchId> MatchCancelled;
    public event Action<MatchId, PlayerId> MatchCompleted;
    public event Action<MatchId> ParticipantJoined;
    public event Action<MatchId> ParticipantLeft;
    public bool IsConnected => _socket?.State == WebSocketState.Open;
    private ClientWebSocket _socket;
    private readonly Uri _endpoint;
    private readonly SemaphoreSlim _sendGate = new(1, 1); // Sets maximum send operations using this resource to (init, max)
    private CancellationTokenSource _connectionLifetimeTokenSource;
    private Task _receiveLoopTask;

    public NodeMatchConnection(Uri endpoint)
    {
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    public async Task ConnectAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        if (IsConnected) throw new InvalidOperationException(nameof(ConnectAsync));

        if (_socket != null)
            await DisconnectAsync(ct);

        ClientWebSocket socket = new();
        _socket = socket;

        try
        {
            await _socket.ConnectAsync(_endpoint, ct);
            JoinMatchMessageDto joinDto = new()
            {
                matchId = matchId.ToString(),
                accessToken = accessToken.Value
            };
            await SendAsync(joinDto, ct);
            string responseJson = await ReceiveTextAsync(ct);
            MatchConnectedMessageDto connectedMsg = RealtimeProtocolParsers.ParseMatchConnected(responseJson, matchId);
            Debug.Log($"Connected to realtime match: {connectedMsg.matchId}\nWith ID: {connectedMsg.playerId}");
            CancellationTokenSource cts = new();
            _connectionLifetimeTokenSource = cts;
            _receiveLoopTask = ReceiveLoopAsync(cts.Token);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);

            if (ReferenceEquals(_socket, socket))
                _socket = null;
            
            socket.Abort();
            socket.Dispose();
            throw ex;
        }
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        CancellationTokenSource cts = _connectionLifetimeTokenSource;
        _connectionLifetimeTokenSource = null;

        if (_socket == null)
        {
            cts?.Cancel();
            _receiveLoopTask = null;
            return;
        }

        ct.ThrowIfCancellationRequested();
        ClientWebSocket socket = _socket;
        _socket = null;
        await _sendGate.WaitAsync(CancellationToken.None);

        try
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                using CancellationTokenSource cancelToken = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cancelToken.CancelAfter(TimeSpan.FromSeconds(2d));

                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: "Disconnecting", cancelToken.Token);
                }
                catch (OperationCanceledException) when (!ct.IsCancellationRequested)
                {
                    Debug.LogWarning("Disconnect handshake timed out. Aborting socket.");
                    socket.Abort();
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Debug.LogException(ex);
        }
        finally
        {
            socket?.Dispose();
            cts?.Dispose();
            _receiveLoopTask = null;
            _sendGate.Release();
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken ct = default)
    {
        while (IsConnected && !ct.IsCancellationRequested)
        {
            string json;
            
            try
            {
                json = await ReceiveTextAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return; // Operation cancelled successfully
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            try
            {
                HandleReceivedMessage(json);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private void HandleReceivedMessage(string json)
    {
        MatchMessageDto msg = JsonUtility.FromJson<MatchMessageDto>(json)
            ?? throw new FormatException("Could not parse JSON");
        
        switch(msg.MessageType)
        {
            case RealtimeProtocol.MessageTypes.MatchStarted:
                if (!Guid.TryParse(msg.matchId, out Guid matchStartedId))
                    throw new FormatException("MatchMessageDto didn't include a valid matchId");

                MatchStarted?.Invoke(new MatchId(matchStartedId));
                Debug.Log("Match Started");
                break;

            case RealtimeProtocol.MessageTypes.MatchCancelled:
                if (!Guid.TryParse(msg.matchId, out Guid matchCancelledId))
                    throw new FormatException("MatchMessageDto didn't include a valid matchId");

                MatchCancelled?.Invoke(new MatchId(matchCancelledId));
                Debug.Log("Match Cancelled");
                break;

            case RealtimeProtocol.MessageTypes.MatchCompleted:
                MatchCompletedMessageDto completeMsg = JsonUtility.FromJson<MatchCompletedMessageDto>(json)
                    ?? throw new FormatException("Unable to parse MatchCompletedMessageDto");

                if (!Guid.TryParse(completeMsg.matchId, out Guid completedMatchId))
                    throw new FormatException("MatchCompletedMessageDto didn't include a valid MatchId");

                if (!Guid.TryParse(completeMsg.winnerPlayerId, out Guid completedWinnerId))
                    throw new FormatException("MatchCompletedMessageDto didn't include a valid PlayerId");

                MatchCompleted?.Invoke(new MatchId(completedMatchId), new PlayerId(completedWinnerId));
                Debug.Log("Match Completed");
                break;

            case RealtimeProtocol.MessageTypes.ParticipantJoined:
                if (!Guid.TryParse(msg.matchId, out Guid participantJoinedId))
                    throw new FormatException("MatchMessageDto didn't include a valid matchId");

                ParticipantJoined?.Invoke(new MatchId(participantJoinedId));
                Debug.Log("Participant Joined");
                break;

            case RealtimeProtocol.MessageTypes.ParticipantLeft:
                if (!Guid.TryParse(msg.matchId, out Guid participantLeftId))
                    throw new FormatException("MatchMessageDto didn't include a valid matchId");

                ParticipantLeft?.Invoke(new MatchId(participantLeftId));
                Debug.Log("Participant Left");
                break;

            case RealtimeProtocol.MessageTypes.Error:
                ErrorMessageDto err = JsonUtility.FromJson<ErrorMessageDto>(json)
                    ?? throw new FormatException("Could not parse Error");
                Debug.LogError($"Error: {err.message}");
                break;

            default:
                throw new ArgumentException($"MessageType was not recognized: {msg.MessageType}");
        }
    }

    private async Task<string> ReceiveTextAsync(CancellationToken ct = default)
    {
        if (!IsConnected)
            throw new InvalidOperationException(nameof(ReceiveTextAsync));

        byte[] buffer = new byte[4096];
        using MemoryStream stream = new();
        ClientWebSocket socket = _socket
            ?? throw new InvalidOperationException("Socket is null!");

        if (socket.State != WebSocketState.Open)
            throw new InvalidOperationException("Socket is not open!");
        
        while (true)
        {
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

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
        finally
        {
            _sendGate.Release();
        }
    }
}
