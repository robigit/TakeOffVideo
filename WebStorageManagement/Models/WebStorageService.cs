using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebStorageManagement.Models;

public class WebStorageService : IWebStorageService
{
    private readonly IJSRuntime? jsRuntime;

    /// <inheritdoc cref="IWebStorageService"/>
    public LocalStorage LocalStorage { get; set; }
    /// <inheritdoc cref="IWebStorageService"/>
    public SessionStorage SessionStorage { get; set; }

    public WebStorageService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
        LocalStorage = new LocalStorage(jsRuntime);
        SessionStorage = new SessionStorage(jsRuntime);
    }
}

