namespace DLB.AspNetCore.BackgroundTasks.Base
{
    public interface IBackgroundDataTask<TData> : IExecuteTask
        where TData : class
    {
        TData Payload { get; set; }
    }
}
