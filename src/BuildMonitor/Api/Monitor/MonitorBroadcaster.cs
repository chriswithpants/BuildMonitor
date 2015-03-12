using System.Threading.Tasks;
using BuildMonitor.Infrastructure.SignalR;

namespace BuildMonitor.Api.Monitor
{
    public class MonitorBroadcaster : Broadcaster, IMonitorBroadcaster
    {
        private readonly IHubContext<MonitorHub> _hubContext;

        public MonitorBroadcaster(IHubContext<MonitorHub> hubContext) 
            : base(hubContext)
        {
            _hubContext = hubContext;
        }

        public Task BuildStatusUpdated(BuildStatusModel model)
        {
            return HubContext.Clients.All.BuildStatusUpdated(model);
        }
    }
}