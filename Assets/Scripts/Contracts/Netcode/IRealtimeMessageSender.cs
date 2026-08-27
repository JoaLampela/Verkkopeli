using System.Threading;
using System.Threading.Tasks;

public interface IRealtimeMessageSender
{
    public Task SendAsync(IRealtimeMessage message, CancellationToken ct = default);
}
