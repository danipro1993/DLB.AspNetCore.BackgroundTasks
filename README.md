# DLB.AspNetCore.BackgroundTasks

DLB.AspNetCore.BackgroundTasks allow generate new task in AspNetCore 2.2+ with new scope. This behavior is useful when you need generate new task and free the actual request.
 
This funcionality is so easy to implement, we only need call a service collection extensions method.

## Requirements

AspNetCore 2.2+
Microsoft.Extentions.Logging

## Installation

DLB.AspNetCore.BackgroundTasks is avaible in nugget gallery.

## Usage

First, we need register necesary dependencies, this is achived calling a method in Startup.cs class.

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
	   //HostedServices and Queue
        services.AddBackgroundQueue();
    }

That is all! If we need generate a new task in background, only need inyect in constructor IBackgroundTaskQueue interface.

Next we bring two examples:

1) Task with additional parameters
2) Task without parameters
	   
Background Task with parameters

    public UserController(IBackgroundTaskQueue)
    {
	    _taskQueue = taskQueue ?? throw new NullException(..);
    }
    
    [Get]
	public async Task<IActionResult> TestMethod(string info)
	{
		//this block request
		await GetUsersAsync();
		
		// Raise background task
		var payload = new BackgrounData(info);
		_taskQueue.QueueBackgroundTask<BackgroundTask,BackgroundData>(payload);
		
		return Ok();
	}
	
	public class BackgroundTask : IBackgroundDataTask<BackgrounData>
	{
	    public BackgroundData Payload {get; set;}

	    public async Task ExecuteAsync(IServiceScope scope, CancellationToken token)
	    {
	        var userAppService = scope.ServiceProvider.GetService<IUserAppService>();

	        await userAppService.SomeMethod(Payload.Info)
	    }
	}

	public class BackgroundData
	{
	    public BackgroundData(string info)
	    {
		    Info = info
	    }
	    
	    public string Info {get; set;}
	}

Background task without parameters

    public UserController(IBackgroundTaskQueue taskQueue)
	    {
		    _taskQueue = taskQueue ?? throw new NullException(..);
	    }
	    
	    [Get]
	    public async Task<IActionResult> TestMethod(string info)
	    {
		    //this block request
		    await GetUsersAsync();
		    
		    // Raise background task
		    _taskQueue.QueueBackgroundTask<SimpleBackgroundTask>();
		    
		    return Ok();
	    }
    }

	public class SimpleBackgroundTask : IBackgroundTask
	{
		public async Task ExecuteAsync(IServiceScope scope, CancellationToken token)
        {
            var userAppService = scope.ServiceProvider.GetService<IUserAppService>();

            await userAppService.SomeMethod(Payload.Info)
        }
	}
