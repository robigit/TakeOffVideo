using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TakeOffVideo;
using TakeOffVideo.Library.TOVFileManagerNS;
//using TakeOffVideo.Library.VideoFileManager;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddSingleton<IVideoFileManager, VideoFileManager>();

builder.Services.AddScoped<TOVFileManager>();

builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options => {
    //List<CultureInfo> supportedCultures = new List<CultureInfo>
    //{
    //    new CultureInfo("en-US"),
    //    new CultureInfo("it-IT"),
    //};
    options.DefaultRequestCulture = new RequestCulture("en-US");
    //options.SupportedCultures = supportedCultures;
    //options.SupportedUICultures = supportedCultures;
});



//CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
//CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");



await builder.Build().RunAsync();
