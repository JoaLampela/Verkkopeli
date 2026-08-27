using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkSettingsSO", menuName = "Verkkopeli/NetworkSettingsSO")]
public sealed class NetworkSettingsSO : ScriptableObject
{
    [SerializeField] private string _baseUrl;
    public string ApiBaseUrl => _baseUrl.TrimEnd('/');
    public Uri RealtimeUri
    {
        get
        {
            Uri baseUri = new(_baseUrl);
            UriBuilder builder = new(baseUri)
            {
                Scheme = baseUri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws",
                Path = AppConstants.Gameplay.RealtimePath
            };
            return builder.Uri;
        }
    }
}
