using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFramework
{
    public interface IServiceRunner
    {
        Task RunAsync(IRunnerContext context);
    }
}