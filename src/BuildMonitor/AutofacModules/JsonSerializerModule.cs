using Autofac;
using BuildMonitor.Infrastructure;
using Newtonsoft.Json;

namespace BuildMonitor.AutofacModules
{
    public class JsonSerializerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance<JsonSerializer>(new CustomJsonSerializer())
                .SingleInstance();
        }
    }
}