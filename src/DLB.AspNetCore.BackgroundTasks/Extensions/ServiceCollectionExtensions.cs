using DLB.AspNetCore.BackgroundTasks.HostedServices;
using DLB.AspNetCore.BackgroundTasks.TaskQueue;
using Microsoft.Extensions.DependencyInjection;

namespace DLB.AspNetCore.BackgroundTasks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBackgroundQueue(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
