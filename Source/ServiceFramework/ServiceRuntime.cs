using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public class ServiceRuntime : IServiceRuntime
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private readonly Lazy<IList<IServiceProcess>> _processors;
        private readonly IServiceResolver _resolver;

        private int _activeProcesses;

        public ServiceRuntime(IServiceResolver resolver)
        {
            _resolver = resolver;
            _processors = new Lazy<IList<IServiceProcess>>(CreateProcesses);
        }

        public bool IsBusy => _activeProcesses > 0;

        public int ActiveProcesses => _activeProcesses;

        public string Name { get; set;  }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IList<IServiceProcess> Processors => _processors.Value;


        public void Start()
        {
            _activeProcesses = 0;

            foreach (var process in _processors.Value)
            {
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    _logger.Error().Message("Error trying to start processor '{0}'.", process.Name).Write();
                }
            }
        }

        public void Stop(Action<int> requestTime = null)
        {
            if (!_processors.IsValueCreated)
                return;

            foreach (var monitor in _processors.Value)
                monitor.Stop();

            // Safe shutdown, service control will wait 30 sec for response.  Can request additional time up to 125 sec.
            // Service Control Handler Function
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms685149(v=vs.85).aspx

            var timeout = DateTimeOffset.Now.AddSeconds(125);
            while (IsBusy && timeout > DateTimeOffset.Now)
            {
                // request additional stop time
                requestTime?.Invoke(500);

                //log active count
                Thread.Sleep(500);
            }
        }


        IDisposable IServiceActivity.BeginWork()
        {
            Interlocked.Increment(ref _activeProcesses);

            var serviceActivity = this as IServiceActivity;
            return new DisposableAction(serviceActivity.EndWork);
        }

        void IServiceActivity.EndWork()
        {
            Interlocked.Decrement(ref _activeProcesses);
        }


        private IList<IServiceProcess> CreateProcesses()
        {
            return _resolver.CreateProcesses(this);
        }

        public override string ToString()
        {
            return $"Processors: ({Processors?.Count})";
        }
    }
}
