using System;

namespace BuildMonitor.Api.Monitor
{
    public class BuildStatusModel
    {
        public string Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public bool Result { get; set; }
    }
}