using UnityEngine;

public sealed class RigidbodyPlayerMotor : MonoBehaviour, IPlayerMotor
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerMotorSettingsSO _settingsSO;
    private uint _remainingJumps;
    private Vector2 _moveIntent;
    private float _bodyYawDegrees;
    private bool _jumpQueued;

    private void Awake()
    {
        ResetJumps();
    }

    private void FixedUpdate()
    {
        HandleMove();
        HandleJumping();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HitJumpResetLayer(other)) return;

        ResetJumps();
    }

    public void SetMovementIntent(Vector2 moveInput, float bodyYawDeg)
    {
        _moveIntent = Vector2.ClampMagnitude(moveInput, 1f);
        _bodyYawDegrees = bodyYawDeg;
    }

    public void RequestJump()
    {
        if (_remainingJumps <= 0) return;

        _jumpQueued = true;
    }

    private void HandleMove()
    {
        _rb.angularVelocity = Vector3.zero;

        Vector3 localMovement = new(_moveIntent.x, 0f, _moveIntent.y);
        Vector3 worldMovement = Quaternion.Euler(0f, _bodyYawDegrees, 0f) * localMovement;

        _rb.linearVelocity = new Vector3
        (
            worldMovement.x * _settingsSO.MoveSpeed,
            _rb.linearVelocity.y,
            worldMovement.z * _settingsSO.MoveSpeed
        );
    }

    private void HandleJumping()
    {
        if (!_jumpQueued || _remainingJumps == 0) return;

        _jumpQueued = false;
        _remainingJumps--;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(_settingsSO.JumpForce * Vector3.up, ForceMode.Impulse);
    }

    private void ResetJumps()
    {
        _remainingJumps = _settingsSO.MaxJumps;
    }

    private bool HitJumpResetLayer(Collider other)
    {
        return (_settingsSO.JumpResetLayer.value & (1 << other.gameObject.layer)) != 0;
    }
}
