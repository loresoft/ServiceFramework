using System;

namespace ServiceFramework
{
    public interface IServiceWorker : IServiceActivity
    {
        IServiceConfiguration Configuration { get; }

        string Name { get; }
        string Status { get; }

        void Start();
        void Stop();

        void ReportStatus(string message);
    }
}