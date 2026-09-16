using System.Threading;
using System.Threading.Tasks;

public interface IAuthenticatedSceneRouter
{
    public Task RouteAsync(CancellationToken ct = default);
}
