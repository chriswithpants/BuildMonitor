using ConfigInjector;
using Serilog.Events;

namespace BuildMonitor.ConfigurationSettings
{
    public class MinimumLogLevel : ConfigurationSetting<LogEventLevel>
    {
    }
}