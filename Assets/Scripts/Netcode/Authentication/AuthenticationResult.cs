public readonly struct AuthenticationResult
{
    public AccessToken AccessToken { get; }

    public AuthenticationResult(AccessToken accessToken)
    {
        AccessToken = accessToken;
    }
}
