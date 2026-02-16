using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<GameDataService>();
builder.Services.AddScoped<IAppStateService, AppStateService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddSingleton<IToastService, ToastService>();
builder.Services.AddSingleton<LoadingService>();
builder.Services.AddScoped<DemoPlansService>();

await builder.Build().RunAsync();
