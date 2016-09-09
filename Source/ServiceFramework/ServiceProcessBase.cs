using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public abstract class ServiceProcessBase<TConfiguration> : IServiceProcess
        where TConfiguration : IServiceConfiguration
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private readonly IServiceRuntime _runtime;
        private readonly TConfiguration _configuration;
        private readonly Lazy<IList<IServiceWorker>> _workers;
        

        private int _activeProcesses;

        protected ServiceProcessBase(IServiceRuntime runtime, TConfiguration configuration)
        {
            _configuration = configuration;
            _runtime = runtime;
            _workers = new Lazy<IList<IServiceWorker>>(CreateWorkers);
        }

        public string Name => _configuration.Name;

        public bool IsBusy => _activeProcesses > 0;

        public int ActiveProcesses => _activeProcesses;

        public TConfiguration Configuration => _configuration;

        IServiceConfiguration IServiceProcess.Configuration => _configuration;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IList<IServiceWorker> Workers => _workers.Value;


        public void Start()
        {
            // Start workers
            foreach (var worker in _workers.Value)
            {
                _logger.Trace()
                    .Message("Starting worker '{0}'.", worker.Name)
                    .Write();

                worker.Start();
            }
        }

        public void Stop()
        {
            if (!_workers.IsValueCreated)
                return;

            // Stop Works
            foreach (var worker in _workers.Value)
            {
                _logger.Trace()
                    .Message("Stopping worker '{0}'.", worker.Name)
                    .Write();

                worker.Stop();
            }

        }


        IDisposable IServiceActivity.BeginWork()
        {
            _runtime.BeginWork();
            Interlocked.Increment(ref _activeProcesses);

            var serviceActivity = this as IServiceActivity;
            return new DisposableAction(serviceActivity.EndWork);
        }

        void IServiceActivity.EndWork()
        {
            Interlocked.Decrement(ref _activeProcesses);
            _runtime.EndWork();
        }


        protected abstract IList<IServiceWorker> CreateWorkers();

        public override string ToString()
        {
            return $"Process: '{Name}', Workers: ({Workers?.Count})";
        }

    }
}