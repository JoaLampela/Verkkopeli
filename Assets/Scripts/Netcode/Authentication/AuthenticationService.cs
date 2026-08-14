using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class AuthenticationService : IAuthenticationService
{
    public bool IsAuthenticated => _currentAuth != null;
    private IAuthenticationClient _authClient;
    private IPlayerProfileClient _profClient;
    private IAuthenticationSessionStore _authSessionStore;
    private IPlayerProfileSession _profSession;
    private AuthenticationResult? _currentAuth;

    public AuthenticationService(IAuthenticationClient authClient, IPlayerProfileClient profClient, IAuthenticationSessionStore authSessionStore, IPlayerProfileSession profSession)
    {
        _authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
        _profClient = profClient ?? throw new ArgumentNullException(nameof(profClient));
        _authSessionStore = authSessionStore ?? throw new ArgumentNullException(nameof(authSessionStore));
        _profSession = profSession ?? throw new ArgumentNullException(nameof(profSession));
    }

    public async Task LoginAsync(string email, string password, CancellationToken ct = default)
    {
        AuthenticationResult auth = await _authClient.LoginAsync(email, password, ct);
        PlayerProfile prof = await _profClient.GetMyProfileAsync(auth.AccessToken, ct);
        await _authSessionStore.SaveAsync(auth, ct);
        ct.ThrowIfCancellationRequested();
        _currentAuth = auth;
        _profSession.SetCurrentProfile(prof);
    }

// Needs bespoke handling of all different failure states. Currently: Server down => Login
    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        AuthenticationResult? storedAuth = await _authSessionStore.LoadAsync(ct);

        if (storedAuth == null) return false;

        try
        {
            PlayerProfile profile = await _profClient.GetMyProfileAsync(storedAuth.Value.AccessToken, ct);
            ct.ThrowIfCancellationRequested();
            _currentAuth = storedAuth;
            _profSession.SetCurrentProfile(profile);
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError("Session Restoration Cancelled");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            await _authSessionStore.ClearAsync(ct);
            _currentAuth = null;
            _profSession.ClearCurrentProfile();
            return false;
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred: {ex}");
            return false;
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        await _authSessionStore.ClearAsync(ct);
        _currentAuth = null;
        _profSession.ClearCurrentProfile();
    }

    public bool TryGetAccessToken(out AccessToken accessToken)
    {
        if (_currentAuth.HasValue)
        {
            accessToken = _currentAuth.Value.AccessToken;
            return true;
        }

        accessToken = default;
        return false;
    }
}
