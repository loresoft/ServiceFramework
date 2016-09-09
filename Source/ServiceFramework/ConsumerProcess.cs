using System;
using System.Collections.Generic;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public class ConsumerProcess : ServiceProcessBase<IConsumerConfiguration>
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        public ConsumerProcess(IServiceRuntime runtime, IConsumerConfiguration configuration)
            : base(runtime, configuration)
        {
        }

        protected override IList<IServiceWorker> CreateWorkers()
        {
            var workers = new List<IServiceWorker>();
            var machineName = Environment.MachineName;

            for (var i = 0; i < Configuration.WorkerCount; i++)
            {
                var name = $"{machineName}.{Configuration.Name}.{i:00}";

                var worker = new ConsumerWorker(this, Configuration, name);
                workers.Add(worker);

                _logger.Trace()
                    .Message("Created consumer worker '{0}'.", worker.Name)
                    .Write();
            }
            return workers;
        }

        public override string ToString()
        {
            return $"Consumer: '{Name}', Workers: ({Workers?.Count})";
        }

    }
}