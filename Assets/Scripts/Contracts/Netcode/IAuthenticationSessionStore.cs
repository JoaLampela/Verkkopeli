using System.Threading;
using System.Threading.Tasks;

public interface IAuthenticationSessionStore
{
    public Task<AuthenticationResult?> LoadAsync(CancellationToken ct = default);
    public Task SaveAsync(AuthenticationResult authResult, CancellationToken ct = default);
    public Task ClearAsync(CancellationToken ct = default);
}
