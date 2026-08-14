using System.Threading;
using System.Threading.Tasks;

public interface IAuthenticationClient
{
    public Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken ct = default);
}
