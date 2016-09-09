using System;
using System.Collections.Generic;

namespace ServiceFramework
{
    public interface IServiceRuntime : IServiceActivity
    {
        string Name { get; set; }
        IList<IServiceProcess> Processors { get; }

        void Start();
        void Stop(Action<int> requestTime);
    }
}