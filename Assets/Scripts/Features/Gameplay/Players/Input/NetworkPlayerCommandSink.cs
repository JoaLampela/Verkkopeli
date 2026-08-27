using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class NetworkPlayerCommandSink : IPlayerCommandSink
{
    private readonly IRealtimeMessageSender _messageSender;
    private readonly CancellationToken _lifetimeToken;
    private PlayerCommandMessageDto _pendingMsg;
    private bool _hasPendingJump;
    private bool _hasPendingMessage;
    private bool _isSending;

    public NetworkPlayerCommandSink(IRealtimeMessageSender messageSender, CancellationToken ct)
    {
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _lifetimeToken = ct;
    }

    public void Submit(in PlayerCommand playerCommand)
    {
        PlayerCommandMessageDto msg = playerCommand.ToDto();
        _hasPendingJump |= msg.jumpPressed;
        msg.jumpPressed = _hasPendingJump;
        _pendingMsg = msg;
        _hasPendingMessage = true;

        if (_isSending) return;

        _ = SendAsync();
    }

    private async Task SendAsync()
    {
        if (_isSending) return;

        _isSending = true;

        try
        {
            while (_hasPendingMessage && !_lifetimeToken.IsCancellationRequested)
            {
                PlayerCommandMessageDto msg = _pendingMsg;
                _hasPendingMessage = false;
                _hasPendingJump = false;
                await _messageSender.SendAsync(msg, _lifetimeToken);
            }
        }
        catch (OperationCanceledException) when (_lifetimeToken.IsCancellationRequested)
        {
            // This block is only executed when the Gameplay scene is being destroyed
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            _isSending = false;

            // Edge-case handling for while loop
            if (_hasPendingMessage && !_lifetimeToken.IsCancellationRequested)
                _ = SendAsync();
        }
    }
}
