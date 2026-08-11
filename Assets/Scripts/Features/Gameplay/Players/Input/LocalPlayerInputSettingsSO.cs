using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LocalPlayerInputSettingsSO", menuName = "Verkkopeli/LocalPlayerInputSettingsSO")]
public sealed class LocalPlayerInputSettingsSO : ScriptableObject
{
    [SerializeField] [Range(0f, 1f)] private float _mouseLookSensitivity;
    [SerializeField] [Range(0f, 360f)] private float _gamepadLookSpeed;
    [SerializeField] private bool _invertVerticalLook;
    [SerializeField] private bool _lockCursor;
    public float MouseLookSensitivity => _mouseLookSensitivity;
    public float GamepadLookSpeed => _gamepadLookSpeed;
    public bool InvertVerticalLook => _invertVerticalLook;
    public bool LockCursor => _lockCursor;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_mouseLookSensitivity == 0f || _gamepadLookSpeed == 0f)
            Debug.LogWarning("Zero-valued variables found in LocalPlayerInputSettingsSO.");
    }
#endif
}
