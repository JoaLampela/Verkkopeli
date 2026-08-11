using System.Threading;
using System.Threading.Tasks;

public interface IMatchResultSink
{
    public Task HandleResultAsync(MatchResult matchResult, CancellationToken ct = default);
}
