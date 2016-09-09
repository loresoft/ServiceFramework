using System;

namespace ServiceFramework
{
    public interface IServiceWorker : IServiceActivity
    {
        IServiceConfiguration Configuration { get; }

        string Name { get; }

        void Start();
        void Stop();
    }
}