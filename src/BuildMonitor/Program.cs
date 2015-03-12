using System;
using BuildMonitor.ConfigurationSettings;
using BuildMonitor.Infrastructure.Environments;
using ConfigInjector.QuickAndDirty;
using Serilog;
using Serilog.Extras.Topshelf;
using Topshelf;

namespace BuildMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = ConfigureLogging();

            var serviceName = DefaultSettingsReader.Get<ServiceName>();
            var serviceDescription = DefaultSettingsReader.Get<ServiceDescription>();

            HostFactory.Run(x =>                                 
            {
                x.Service<BuildMonitorService>();
                
                x.RunAsLocalSystem();
                x.UseSerilog(logger);

                x.SetDescription(serviceDescription);
                x.SetServiceName(serviceName);                       
            });                                                  
        }

        private static ILogger ConfigureLogging()
        {
            var minimumLogLevel = DefaultSettingsReader.Get<MinimumLogLevel>();
            var serverUrl = DefaultSettingsReader.Get<SeqServerUri>().ToString();

            var loggerConfig = new LoggerConfiguration()
                   .MinimumLevel.Is(minimumLogLevel)
                   .Enrich.FromLogContext()
                   .Enrich.WithMachineName()
                   .Enrich.WithThreadId()
                   .Enrich.WithProperty("ApplicationName", "BuildMonitor")
                   .Enrich.WithProperty("ApplicationVersion", typeof(BuildMonitorService).Assembly.GetName().Version)
                   .Enrich.WithProperty("EnvironmentType", AppEnvironment.EnvironmentType)
                   .Enrich.WithProperty("EnvironmentName", AppEnvironment.EnvironmentName)
                   .Enrich.WithProperty("ServiceAccount", Environment.UserName)
                   .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                   .WriteTo.Seq(serverUrl)
                   .WriteTo.Trace();

            return loggerConfig.CreateLogger();
        }
    }
}
