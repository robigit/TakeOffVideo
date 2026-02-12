using Microsoft.JSInterop;

namespace TakeOffVideo.Library.Util;

/// <summary>
/// Helper for loading any JavaScript (ES6) module and calling its exports
/// </summary>
public abstract class JSModule : IAsyncDisposable
{
    private readonly Task<IJSObjectReference?> moduleTask;
    private bool _moduleLoadFailed = false;

    // On construction, we start loading the JS module
    protected JSModule(IJSRuntime js, string moduleUrl)
    {
        moduleTask = Task.Run(async () =>
        {
            try
            {
                return await js.InvokeAsync<IJSObjectReference>("import", moduleUrl);
            }
            catch (JSException)
            {
                _moduleLoadFailed = true;
                return null;
            }
        });
    }

    // Methods for invoking exports from the module
    protected async ValueTask InvokeVoidAsync(string identifier, params object[]? args)
    {
        var module = await moduleTask;
        if (module == null || _moduleLoadFailed)
            throw new InvalidOperationException($"JavaScript module failed to load. Method '{identifier}' cannot be invoked.");
        
        await module.InvokeVoidAsync(identifier, args);
    }

    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object[]? args)
    {
        var module = await moduleTask;
        if (module == null || _moduleLoadFailed)
            throw new InvalidOperationException($"JavaScript module failed to load. Method '{identifier}' cannot be invoked.");
        
        return await module.InvokeAsync<T>(identifier, args);
    }

    // On disposal, we release the JS module
    public async ValueTask DisposeAsync()
    {
        var module = await moduleTask;
        if (module != null)
            await module.DisposeAsync();
    }
}
