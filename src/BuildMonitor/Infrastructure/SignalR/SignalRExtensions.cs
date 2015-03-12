using Autofac;
using BuildMonitor.Infrastructure.Environments;
using Microsoft.AspNet.SignalR;
using Owin;

namespace BuildMonitor.Infrastructure.SignalR
{
    public static class SignalRExtensions
    {
        public static void UseSignalR(this IAppBuilder app, IContainer container)
        {
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());

            app.MapSignalR("/signalr", new HubConfiguration
            {
                EnableDetailedErrors = !AppEnvironment.IsProduction(),
                EnableJSONP = false,
                EnableJavaScriptProxies = false
            });

            //GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}