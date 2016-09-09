using System;
using System.Collections.Generic;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public class PollingProcess : ServiceProcessBase<IPollingConfiguration>
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        public PollingProcess(IServiceRuntime runtime, IPollingConfiguration configuration) 
            : base(runtime, configuration)
        {
        }

        protected override IList<IServiceWorker> CreateWorkers()
        {
            var workers = new List<IServiceWorker>();

            int count = Configuration.WorkerCount;

            for (int i = 0; i < count; i++)
            {
                string name = $"{Configuration.Name}-Worker{i:00}";
                var worker = new PollingWorker(this, Configuration, name);

                _logger.Trace()
                    .Message("Created polling worker '{0}'.", worker.Name)
                    .Write();

                workers.Add(worker);
            }

            return workers;
        }

        public override string ToString()
        {
            return $"Poller: '{Name}', Workers: ({Workers?.Count})";
        }


    }
}