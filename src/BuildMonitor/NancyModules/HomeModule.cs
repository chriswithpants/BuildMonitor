using Nancy;

namespace BuildMonitor.NancyModules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => View[@"index"];
        }
    }
}