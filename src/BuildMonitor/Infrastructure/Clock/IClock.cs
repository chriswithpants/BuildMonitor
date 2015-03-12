using System;

namespace BuildMonitor.Infrastructure.Clock
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }
}