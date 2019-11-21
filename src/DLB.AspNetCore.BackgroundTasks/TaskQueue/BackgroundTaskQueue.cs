using DLB.AspNetCore.BackgroundTasks.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.AspNetCore.BackgroundTasks.TaskQueue
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<IServiceScope, CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<IServiceScope, CancellationToken, Task>>();

        private ConcurrentQueue<IExecuteTask> _workTask =
            new ConcurrentQueue<IExecuteTask>();

        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<IServiceScope, CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public void QueueBackgroundTask<TBackgroundTask, TData>(TData payload)
            where TData : class
            where TBackgroundTask : IBackgroundDataTask<TData>
        {
            var instance = (IBackgroundDataTask<TData>)Activator.CreateInstance(typeof(TBackgroundTask));

            instance.Payload = payload;

            _workTask.Enqueue(instance);
            _signal.Release();
        }

        public void QueueBackgroundTask<TBackgroundTask>()
            where TBackgroundTask : IBackgroundTask
        {
            var instance = (IBackgroundTask)Activator.CreateInstance(typeof(TBackgroundTask));

            _workTask.Enqueue(instance);
            _signal.Release();
        }

        public async Task<IExecuteTask> DequeueAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            _workTask.TryDequeue(out var backgroundTask);

            return backgroundTask;
        }
    }
}
