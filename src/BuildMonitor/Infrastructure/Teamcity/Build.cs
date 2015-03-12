using System.Collections.Generic;

namespace BuildMonitor.Infrastructure.Teamcity
{
    public class Build
    {
        public string BuildStatus { get; set; }
        public string BuildResult { get; set; }
        public string BuildResultPrevious { get; set; }
        public string BuildResultDelta { get; set; }
        public string NotifyType { get; set; }
        public string BuildFullName { get; set; }
        public string BuildName { get; set; }
        public string BuildId { get; set; }
        public string BuildTypeId { get; set; }
        public string BuildInternalTypeId { get; set; }
        public string BuildExternalTypeId { get; set; }
        public string BuildStatusUrl { get; set; }
        public string BuildStatusHtml { get; set; }
        public string RootUrl { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectInternalId { get; set; }
        public string ProjectExternalId { get; set; }
        public string BuildNumber { get; set; }
        public string AgentName { get; set; }
        public string AgentOs { get; set; }
        public string AgentHostname { get; set; }
        public string TriggeredBy { get; set; }
        public string Message { get; set; }
        public string Text { get; set; }
        public string BuildStateDescription { get; set; }
        public IList<string> BuildRunners { get; set; }
        public IList<object> ExtraParameters { get; set; }
        public IList<TeamcityProperty> TeamcityProperties { get; set; }
    }
}