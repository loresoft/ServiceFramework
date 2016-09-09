using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFramework
{
    public interface IRunnerContext
    {
        CancellationToken Cancellation { get; }

        IServiceConfiguration Configuration { get; }

        IServiceWorker Worker { get; }
    }
}
