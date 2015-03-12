using Autofac;
using BuildMonitor.Infrastructure.SignalR;

namespace BuildMonitor.AutofacModules
{
    public class SignalRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterHubs(ThisAssembly);

            builder.RegisterGeneric(typeof(HubContext<>))
                .As(typeof(IHubContext<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<Broadcaster>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}



    
