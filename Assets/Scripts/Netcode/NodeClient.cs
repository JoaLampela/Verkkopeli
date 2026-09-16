using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public sealed class NodeClient : IAuthenticationClient, IPlayerProfileClient, IMatchClient
{
    private readonly string _baseUrl;

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

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
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

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
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

    public async Task<MatchInfo> CreateMatchAsync(AccessToken accessToken, CancellationToken ct = default)
    {
        using UnityWebRequest request = new(_baseUrl + AppConstants.Api.MatchPath, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        MatchInfo info = JsonUtility.FromJson<MatchResponseDto>(request.downloadHandler.text).FromDto();
        return info;
    }

    public async Task<MatchInfo> GetMatchAsync(MatchId matchId, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/{matchId.Guid}";
        using UnityWebRequest request = UnityWebRequest.Get(url);
        await SendAsync(request, ct);

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");

        MatchInfo info = JsonUtility.FromJson<MatchResponseDto>(request.downloadHandler.text).FromDto();
        return info;
    }

    public async Task<MatchInfo> JoinMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/{matchId.Guid}/join";
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");

        MatchInfo info = JsonUtility.FromJson<MatchResponseDto>(request.downloadHandler.text).FromDto();
        return info;
    }

    public async Task LeaveMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/{matchId.Guid}/leave";
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        Debug.Log($"Left match with matchId={matchId}");
    }

    public async Task StartMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/{matchId.Guid}/start";
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        Debug.Log($"Started match with MatchId={matchId}");
    }

    public async Task CompleteMatchAsync(MatchId matchId, AccessToken accessToken, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/{matchId.Guid}/complete";
        using UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");
        
        Debug.Log($"Completed match with MatchId={matchId}");
    }

    public async Task<MatchInfo?> GetActiveMatchAsync(AccessToken accessToken, CancellationToken ct = default)
    {
        string url = $"{_baseUrl}{AppConstants.Api.MatchPath}/active";
        using UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {accessToken.Value}");
        await SendAsync(request, ct);

        if (request.responseCode == (long)HttpStatusCode.NoContent)
            return null;

        if (request.responseCode == (long)HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Authentication rejected");

        if (request.result != UnityWebRequest.Result.Success)
            throw new InvalidOperationException($"Request failed with code {request.responseCode}: {request.downloadHandler.text}");

        MatchInfo info = JsonUtility.FromJson<MatchResponseDto>(request.downloadHandler.text).FromDto();
        return info;
    }
}
