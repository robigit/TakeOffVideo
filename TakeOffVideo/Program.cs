using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TakeOffVideo;
using TakeOffVideo.Library.RLogger;
using TakeOffVideo.Library.VideoFileManager;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<IVideoFileManager, VideoFileManager>();

builder.Services.AddSingleton<IRLogger, RLogger>();

await builder.Build().RunAsync();
