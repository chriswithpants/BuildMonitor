using BuildMonitor.Infrastructure.SignalR;
using Newtonsoft.Json;

namespace BuildMonitor.Infrastructure
{
    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            ContractResolver = new SignalRContractResolver();
            Formatting = Formatting.Indented;
            DefaultValueHandling = DefaultValueHandling.Include;
            DateParseHandling = DateParseHandling.DateTimeOffset;
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            
        }
    }
}