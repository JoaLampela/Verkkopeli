using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSettingsSO", menuName = "Verkkopeli/NetworkSettingsSO")]
public sealed class NetworkSettingsSO : ScriptableObject
{
    [SerializeField] private string _baseUrl;
    public string BaseUrl => _baseUrl.TrimEnd('/');
}
