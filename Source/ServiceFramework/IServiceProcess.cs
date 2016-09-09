using System;
using System.Collections.Generic;

namespace ServiceFramework
{
    public interface IServiceProcess : IServiceActivity
    {
        IServiceConfiguration Configuration { get; }

        IList<IServiceWorker> Workers { get; }

        string Name { get; }

        void Start();
        void Stop();
    }
}