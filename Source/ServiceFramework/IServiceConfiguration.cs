using System;

namespace ServiceFramework
{
    public interface IServiceConfiguration
    {
        string Name { get; }
        int WorkerCount { get; }
        bool KeepAlive { get; }

        IServiceRunner CreateRunner(IServiceWorker worker);
    }
}