using Autofac;
using BuildMonitor.Infrastructure;

namespace BuildMonitor.AutofacModules
{
    public class LookupServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<ILookupService>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
        }
    }
}