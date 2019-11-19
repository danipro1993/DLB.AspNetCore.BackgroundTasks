using DLB.AspNetCore.BackgroundTasks.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.AspNetCore.BackgroundTasks.TaskQueue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundTask<TBackgroundTask, TData>(TData payload)
                where TData : class
                where TBackgroundTask : IBackgroundDataTask<TData>;

        void QueueBackgroundTask<TBackgroundTask>()
                where TBackgroundTask : IBackgroundTask;

        Task<IExecuteTask> DequeueAsync(IServiceScope scope, CancellationToken cancellationToken);
    }
}
