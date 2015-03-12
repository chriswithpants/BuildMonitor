using Autofac;
using BuildMonitor.Infrastructure.Clock;

namespace BuildMonitor.AutofacModules
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemClock>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}