using Microsoft.AspNet.SignalR;

namespace BuildMonitor.Infrastructure.SignalR
{
    public abstract class Broadcaster
    {
        protected IHubContext HubContext { get; set; }

        protected Broadcaster(IHubContext hubContext)
        {
            HubContext = hubContext;
        }
    }
}