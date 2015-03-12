using System.IO;
using BuildMonitor.Infrastructure.Clock;
using BuildMonitor.Infrastructure.Teamcity;
using Nancy;
using Newtonsoft.Json;

namespace BuildMonitor.Api.Monitor
{
    public class MonitorModule : NancyModule
    {
        private readonly IClock _clock;
        private readonly IBuildStatusLookup _lookup;

        public MonitorModule(IClock clock, IBuildStatusLookup lookup)
        {
            _clock = clock;
            _lookup = lookup;
            Get["/api/buildstatuses", true] = async (args, ct) =>
            {
                var buildStatuses = await _lookup.GetBuildStatuses();
                var response = Response.AsJson(buildStatuses);
                response.StatusCode = HttpStatusCode.Accepted;

                return response;
            };
            
            Post["/api/buildstatuses", true] = async (args, ct) =>
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var request = JsonConvert.DeserializeObject<Teamcity>(body);

                    var model = new BuildStatusModel
                    {
                        Id = request.Build.BuildId,
                        Time = _clock.UtcNow,
                        Heading = string.Format("{0} :: {1}", request.Build.ProjectName, request.Build.BuildName),
                        Text = request.Build.BuildStatus,
                        Link = request.Build.BuildStatusUrl,
                        Result = request.Build.BuildResult == "success"
                    };

                    await _lookup.AddOrUpdate(model);
                }

                var response = Response.AsText("");
                response.StatusCode = HttpStatusCode.Accepted;

                return response;
            };
        }
    }
}