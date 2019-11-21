using DLB.AspNetCore.BackgroundTasks.TaskQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.AspNetCore.BackgroundTasks.HostedServices
{
    public class QueuedHostedService : BackgroundService
    {
        private CancellationTokenSource _shutdown =
            new CancellationTokenSource();
        private Task _backgroundTask;
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTaskQueue _taskQueue;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, 
                                   ILogger<QueuedHostedService> logger, 
                                   IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            _backgroundTask = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await BackgroundProcessing();
                }
            }, stoppingToken);

            await _backgroundTask;
        }

        private async Task BackgroundProcessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                using(var scope = _serviceProvider.CreateScope())
                {
                    var workTask = await _taskQueue.DequeueAsync(scope, _shutdown.Token);

                    try
                    {
                        if(workTask != null)
                            await workTask.ExecuteAsync(scope, _shutdown.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error occurred executing {WorkItem}.", nameof(workTask));
                    }
                }                
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            await Task.WhenAny(_backgroundTask,
                    Task.Delay(Timeout.Infinite, stoppingToken));
        }
    }
}
