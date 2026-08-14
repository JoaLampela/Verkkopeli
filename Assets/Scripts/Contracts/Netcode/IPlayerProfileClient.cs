using System.Threading;
using System.Threading.Tasks;

public interface IPlayerProfileClient
{
    Task<PlayerProfile> GetMyProfileAsync(AccessToken accessToken, CancellationToken ct = default);
}
