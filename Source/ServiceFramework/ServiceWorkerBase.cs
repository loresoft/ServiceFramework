using System;
using System.Threading;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public abstract class ServiceWorkerBase<TConfiguration> : IServiceWorker
        where TConfiguration : IServiceConfiguration
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private readonly IServiceProcess _serviceProcess;
        private readonly TConfiguration _configuration;

        private IServiceRunner _runner;
        private int _activeProcesses;

        protected ServiceWorkerBase(IServiceProcess serviceProcess, TConfiguration configuration, string name)
        {
            Name = name;

            _serviceProcess = serviceProcess;
            _configuration = configuration;
        }

        public string Name { get; }

        public bool IsBusy => _activeProcesses > 0;

        public int ActiveProcesses => _activeProcesses;

        public TConfiguration Configuration => _configuration;

        IServiceConfiguration IServiceWorker.Configuration => _configuration;

        public abstract void Start();
        public abstract void Stop();

        public IDisposable BeginWork()
        {
            _serviceProcess.BeginWork();
            Interlocked.Increment(ref _activeProcesses);

            return new DisposableAction(EndWork);
        }

        public void EndWork()
        {
            Interlocked.Decrement(ref _activeProcesses);
            _serviceProcess.EndWork();
        }

        protected virtual IServiceRunner CreateRunner()
        {
            if (!Configuration.KeepAlive)
                return Configuration.CreateRunner(this);

            if (_runner == null)
                _runner = Configuration.CreateRunner(this);

            return _runner;
        }

        protected virtual void FreeRunner()
        {
            if (Configuration.KeepAlive)
                return;

            // dispose runner if possible
            var disposable = _runner as IDisposable;
            disposable?.Dispose();

            _runner = null;
        }


        public override string ToString()
        {
            return $"Worker: '{Name}', Busy: {IsBusy}";
        }

    }
}