using UnityEngine;

public sealed class PlayerLookController : MonoBehaviour, IPlayerLookController
{
    [SerializeField] private Transform _bodyRoot;
    [SerializeField] private Transform _viewRoot;

    public void SetLookDirection(PlayerLookAngles lookAngles)
    {
        float pitch = Mathf.Clamp(lookAngles.PitchDegrees, -89f, 89f);
        _bodyRoot.localRotation = Quaternion.Euler(0f, lookAngles.YawDegrees, 0f);
        _viewRoot.localRotation = Quaternion.Euler(-pitch, 0f, 0f);
    }
}
