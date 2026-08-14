using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public sealed class NodeClient : IAuthenticationClient, IPlayerProfileClient
{
    private string _baseUrl;

    public NodeClient(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException(nameof(baseUrl));

        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        LoginRequestDto dto = new() { email = email, password = password };
        string json = JsonUtility.ToJson(dto);
        using UnityWebRequest request = new(_baseUrl + AppConstants.Api.LoginPath, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await SendAsync(request, ct);

        if (request.responseCode == 401)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        LoginResponseDto responseDto = JsonUtility.FromJson<LoginResponseDto>(request.downloadHandler.text);
        return new AuthenticationResult(new AccessToken(responseDto.accessToken));
    }

    public async Task<PlayerProfile> GetMyProfileAsync(AccessToken accessToken, CancellationToken ct = default)
    {
        using UnityWebRequest request = UnityWebRequest.Get(_baseUrl + AppConstants.Api.ProfilePath);
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == 401)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        PlayerProfile playerProfile = JsonUtility.FromJson<PlayerProfileResponseDto>(request.downloadHandler.text).FromDto();
        return playerProfile;
    }

    private async Task SendAsync(UnityWebRequest request, CancellationToken ct = default)
    {
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            if (ct.IsCancellationRequested)
            {
                request.Abort();
                ct.ThrowIfCancellationRequested();
            }

            await Task.Yield();
        }

        ct.ThrowIfCancellationRequested();
    }
}
