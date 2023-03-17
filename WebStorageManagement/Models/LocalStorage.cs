using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebStorageManagement.Models;

public class LocalStorage : IStorageService
{
    private IJSObjectReference? module = null;
    private readonly IJSRuntime? jsRuntime;

    public LocalStorage(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    /// <inheritdoc cref="IStorageService"/>
    private async Task Init()
    {
        if (module == null)
        {
            module = await jsRuntime.InvokeAsync<IJSObjectReference>
                ("import", "./_content/WebStorageManagement/scripts/localStorage.js");
        }
    }

    /// <inheritdoc cref="IStorageService"/>
    public async Task Save(string keyName, string value)
    {
        await Init();

        if (module is not null)
            await module!.InvokeVoidAsync("BlazorSetLocalStorage", keyName, value);
    }

    /// <inheritdoc cref="IStorageService"/>
    public async Task<string> Read(string keyName)
    {
        await Init();

        if (module is not null)
        {
            return await module!.InvokeAsync<string>("BlazorGetLocalStorage", keyName);
        }
        else
        {
            return string.Empty;
        }
    }

    /// <inheritdoc cref="IStorageService"/>
    public async Task Clear()
    {
        await Init();

        if (module is not null)
            await module!.InvokeVoidAsync("BlazorClearLocalStorage");
    }

}

