using System.Threading.Tasks;

namespace BuildMonitor.Api.Monitor
{
    public interface IMonitorBroadcaster
    {
        Task BuildStatusUpdated(BuildStatusModel model);
    }
}