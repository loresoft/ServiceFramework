using System;
using System.Collections.Generic;

namespace ServiceFramework
{
    public interface IServiceResolver
    {
        IList<IServiceProcess> CreateProcesses(IServiceRuntime serviceRuntime);
    }
}