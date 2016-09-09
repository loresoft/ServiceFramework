using System;
using System.Threading.Tasks;

namespace ServiceFramework
{
    public interface IConsumerConfiguration : IServiceConfiguration
    {
        TimeSpan TimeToLive { get; }
    }
}