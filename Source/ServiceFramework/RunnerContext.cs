using System;
using System.Threading;

namespace ServiceFramework
{
    public class RunnerContext : IRunnerContext
    {
        public RunnerContext(IServiceConfiguration configuration, IServiceWorker worker, CancellationToken cancellationToken)
        {
            Configuration = configuration;
            Worker = worker;
            Cancellation = cancellationToken;
        }

        public CancellationToken Cancellation { get; }

        public IServiceConfiguration Configuration { get; }

        public IServiceWorker Worker { get; }
    }
}