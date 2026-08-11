using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PlayerMotorSettingsSO", menuName = "Verkkopeli/PlayerMotorSettingsSO")]
public sealed class PlayerMotorSettingsSO : ScriptableObject
{
    [SerializeField] [Range(0f, 10f)] private float _moveSpeed;
    [SerializeField] [Range(0f, 10f)] private float _jumpForce;
    [SerializeField] [Range(0, 10)] private uint _maxJumps;
    [SerializeField] private LayerMask _jumpResetLayer;
    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;
    public uint MaxJumps => _maxJumps;
    public LayerMask JumpResetLayer => _jumpResetLayer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_moveSpeed == 0f
        || _jumpForce == 0f
        || _maxJumps == 0)
            Debug.LogWarning("Zero-valued variables found in PlayerMotorSettingsSO.");
    }
#endif
}
