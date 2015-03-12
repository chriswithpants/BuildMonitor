using Autofac;
using BuildMonitor.Infrastructure;
using Serilog;

namespace BuildMonitor.AutofacModules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RequestLogContext>()
                .InstancePerLifetimeScope();

            // we fetch this each time as the instance pointed to by Log.Logger gets changed at startup.
            builder.Register(c => Log.Logger)
                .As<ILogger>()
                .ExternallyOwned();
        }
    }
}