using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ServiceFramework.Logging;

namespace ServiceFramework
{
    public class ConsumerWorker : ServiceWorkerBase<IConsumerConfiguration>
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ServiceRuntime>();

        private volatile bool _isAwaitingShutdown;  //thread safe shutdown flag

        private CancellationTokenSource _cancellation;
        private Task _task;

        private int _errorCount = 0;
        private DateTimeOffset _lastError = DateTimeOffset.MinValue;

        public ConsumerWorker(IServiceProcess serviceProcess, IConsumerConfiguration configuration, string name)
            : base(serviceProcess, configuration, name)
        {
        }

        public override void Start()
        {
            _logger.Trace()
                .Message("Starting consumer '{0}' worker.", Name)
                .Write();

            _isAwaitingShutdown = false;
            StartTask();
        }

        public override void Stop()
        {
            _logger.Trace()
                .Message("Stoping consumer '{0}' worker.", Name)
                .Write();

            _isAwaitingShutdown = true;
            _cancellation?.Cancel();
        }

        private void StartTask()
        {
            // don't start if shutting down
            if (_isAwaitingShutdown || IsBusy)
                return;

            _logger.Info()
                .Message("Consumer Task: {0} is starting.", Name)
                .Write();

            try
            {
                _cancellation = new CancellationTokenSource(Configuration.TimeToLive);

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
                _task = runner.RunAsync(context);
                _task.ContinueWith(EndTask);
                watch.Stop();

                _logger.Trace()
                    .Message("Worker '{0}' started task in: {1} ms.", Name, watch.ElapsedMilliseconds)
                    .Write();
            }
            catch (Exception ex)
            {
                _logger.Error()
                    .Message("Error running '{0}' process. {1}", Name, ex.Message)
                    .Exception(ex)
                    .Write();
            }
        }

        private async Task EndTask(Task task)
        {
            try
            {
                // Task exited with exception
                if (task.IsFaulted)
                {
                    _logger.Warn()
                        .Message("Consumer: {0} has faulted.", Name)
                        .Write();

                    _errorCount++;
                    if ((DateTimeOffset.Now - _lastError).TotalMinutes > 60)
                    {
                        //reset the error count if we've gone an hour without one.
                        _errorCount = 1;
                    }
                    _lastError = DateTimeOffset.Now;

                    var sleepMinutes = _errorCount < 10 ? _errorCount : 10;
                    _logger.Warn()
                        .Message($"{Name} Sleeping for {sleepMinutes} minutes")
                        .Write();

                    await Task.Delay(1000 * 60 * sleepMinutes); // 1 minute per failure, max of 10
                }
                // Task exited, should only be when canceled
                else
                {
                    _logger.Info()
                        .Message("Consumer: {0} has exited.", Name)
                        .Write();
                }

            }
            catch (Exception ex)
            {
                _logger.Error()
                    .Message("Error while processing task end for '{0}'. {1}", Name, ex.Message)
                    .Exception(ex)
                    .Write();
            }
            finally
            {
                Reset();
                StartTask();
            }
        }

        private void Reset()
        {
            FreeRunner();

            _cancellation?.Dispose();
            _task?.Dispose();

            _cancellation = new CancellationTokenSource();
            _task = null;
        }


    }
}