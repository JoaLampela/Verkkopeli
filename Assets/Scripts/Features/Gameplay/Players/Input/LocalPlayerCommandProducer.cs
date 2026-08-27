using System;
using UnityEngine;

public sealed class LocalPlayerCommandProducer : MonoBehaviour
{
    private IPlayerInputSource _playerInputSource;
    private IPlayerCommandSink _playerCommandSink;
    private uint _sequence;
    private float _yaw;
    private float _pitch;
    private bool _isReady;

    public void Bind(IPlayerInputSource playerInputSource, IPlayerCommandSink playerCommandSink)
    {
        _playerInputSource = playerInputSource ?? throw new ArgumentNullException(nameof(playerInputSource));
        _playerCommandSink = playerCommandSink ?? throw new ArgumentNullException(nameof(playerCommandSink));
        _isReady = true;
    }

    private void Update()
    {
        if (!_isReady) return;

        if (!_playerInputSource.TryGetPlayerInputFrame(out PlayerInputFrame frame))
            return;

        PlayerCommand cmd = CreateNewCommand(frame);
        _playerCommandSink.Submit(cmd);
    }

    private PlayerCommand CreateNewCommand(PlayerInputFrame inputFrame)
    {
        _yaw = Mathf.Repeat(_yaw + inputFrame.LookDelta.x, 360f);
        _pitch = Mathf.Clamp(_pitch + inputFrame.LookDelta.y, -89f, 89f);
        _sequence++;
        PlayerLookAngles look = new(_yaw, _pitch);
        return new PlayerCommand(_sequence, inputFrame.MoveDir, look, inputFrame.JumpPressed);
    }
}
