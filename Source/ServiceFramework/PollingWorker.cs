using System;
using System.Diagnostics;
using System.Threading;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public class PollingWorker : ServiceWorkerBase<IPollingConfiguration>
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private readonly Timer _pollTimer;
        private readonly Random _random;

        private volatile bool _isAwaitingShutdown;  //thread safe shutdown flag
        private CancellationTokenSource _cancellation;

        public PollingWorker(IServiceProcess serviceProcess, IPollingConfiguration configuration, string name)
            : base(serviceProcess, configuration, name)
        {
            _random = new Random();
            _pollTimer = new Timer(Run);
            _cancellation = new CancellationTokenSource();
        }


        public override void Start()
        {
            _logger.Trace()
                .Message("Starting '{0}' worker polling at {1}.", Name, Configuration.PollTime)
                .Write();

            _isAwaitingShutdown = false;
            _cancellation = new CancellationTokenSource();

            StartTimer(TimeSpan.FromMilliseconds(500));
        }

        public override void Stop()
        {
            _logger.Trace()
                .Message("Stoping '{0}' worker.", Name)
                .Write();

            _isAwaitingShutdown = true;
            _cancellation.Cancel();

            StopTimer();
        }

        public void Trigger()
        {
            StartTimer(TimeSpan.FromMilliseconds(500));
        }


        private void StartTimer(TimeSpan pollTime)
        {
            // don't start if shutting down
            if (_isAwaitingShutdown || IsBusy)
                return;

            // randomize start time to reduce resource contention with multiple runners.
            int nextRun = (int)pollTime.TotalMilliseconds;
            if (nextRun > 1000)
            {
                int padding = (int)(nextRun * .05);
                int high = nextRun + padding;
                int low = nextRun - padding;
                nextRun = _random.Next(low, high);
            }

            // timer can't be faster then 100 ms
            nextRun = Math.Max(100, nextRun);

            // timer only fires once, up to call back to start timer again
            _pollTimer.Change(nextRun, Timeout.Infinite);
        }

        private void StopTimer()
        {
            _pollTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }


        // called in background thread
        private async void Run(object state)
        {
            try
            {
                //MappedDiagnosticsContext.Set("Worker", Name);
                BeginWork();

                _logger.Trace()
                    .Message("Running '{0}' worker.", Name)
                    .Write();

                var runner = CreateRunner();
                if (runner == null)
                {
                    _logger.Error()
                        .Message("Error creating runner for '{0}'.", Name)
                        .Write();

                    // don't start again
                    _isAwaitingShutdown = true;
                    return;
                }

                var context = new RunnerContext(Configuration, this, _cancellation.Token);

                var watch = Stopwatch.StartNew();
                await runner.RunAsync(context).ConfigureAwait(false);
                watch.Stop();

                _logger.Trace()
                    .Message("Completed '{0}' process in: {1} ms.", Name, watch.ElapsedMilliseconds)
                    .Write();

            }
            catch (Exception ex)
            {
                _logger.Error()
                    .Message("Error running '{0}' process. {1}", Name, ex.Message)
                    .Exception(ex)
                    .Write();
            }
            finally
            {
                // dispose runner if possible
                FreeRunner();

                EndWork();
                //MappedDiagnosticsContext.Remove("Worker");

                StartTimer(Configuration.PollTime);
            }

        }
    }
}