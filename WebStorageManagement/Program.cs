using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebStorageManagement.Models;

namespace WebStorageManagement;
public static class Program
{
    public static IServiceCollection AddWebStorageManagement(this IServiceCollection services)
    {
        services.AddSingleton<IWebStorageService, WebStorageService>();
        return services; ;
    }
}

