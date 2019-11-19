using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace DLB.AspNetCore.BackgroundTasks.Base
{
    public interface IExecuteTask
    {
        Task ExecuteAsync(IServiceScope scope, CancellationToken token);
    }
}
