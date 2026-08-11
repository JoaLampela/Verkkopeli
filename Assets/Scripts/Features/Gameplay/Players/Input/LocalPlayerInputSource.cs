using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class LocalPlayerInputSource : MonoBehaviour, IPlayerInputSource
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private LocalPlayerInputSettingsSO _settingsSO;

    private bool _isEnabled;

    private void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        if (_isEnabled) return;

        Cursor.visible = false;
        Cursor.lockState = _settingsSO.LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
        _moveAction.action.Enable();
        _lookAction.action.Enable();
        _jumpAction.action.Enable();
        _isEnabled = true;
    }

    public void Disable()
    {
        if (!_isEnabled) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _moveAction.action.Disable();
        _lookAction.action.Disable();
        _jumpAction.action.Disable();
        _isEnabled = false;
    }

    public PlayerInputFrame GetPlayerInputFrame()
    {
        if (!_isEnabled) return default;

        Vector2 move = _moveAction.action.ReadValue<Vector2>();
        Vector2 look = GetLookValue();
        bool jump = _jumpAction.action.WasPressedThisFrame();

        return new PlayerInputFrame(move, look, jump);
    }

    private Vector2 GetLookValue()
    {
        Vector2 rawValue = _lookAction.action.ReadValue<Vector2>();

        Vector2 look = _lookAction.action.activeControl?.device switch
        {
            Gamepad => rawValue * _settingsSO.GamepadLookSpeed * Time.deltaTime, // Consider Time.unscaledDeltaTime
            Mouse => rawValue * _settingsSO.MouseLookSensitivity, // Already time-scaled
            null => Vector2.zero,
            _ => throw new ArgumentException("Unsupported control scheme!")
        };

        if (_settingsSO.InvertVerticalLook) look.y *= -1;

        return look;
    }
}
