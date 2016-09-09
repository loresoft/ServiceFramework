using System;

namespace ServiceFramework
{
    public interface IPollingConfiguration : IServiceConfiguration
    {
        TimeSpan PollTime { get; }
    }
}