using Blazored.LocalStorage;
using BlazorStrap;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QualityStation.Shared.Pages.Providers;
using QualityStation.Shared.Pages.Services.AuthorizedHttpClientService;
using QualityStation.Shared.Pages.Services.RecordLoadingService;
using QualityStation.Shared.Pages.Services.StorageService;
using QualityStation.Shared.Pages.ViewModels;
using QualityStation.WebClient;
using QualityStation.WebClient.Services.StorageService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorStrap();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5149")
});
builder.Services.AddScoped<LoginPageViewModel>();
builder.Services.AddScoped<ProfilePageViewModel>();
builder.Services.AddScoped<StationPageViewModel>();
builder.Services.AddTransient<AttributeComponentViewModel>();
builder.Services.AddScoped<SingleStationPageViewModel>();
builder.Services.AddTransient<SingleStationChartViewModel>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddTransient<AuthenticationStateProvider, CustomAuthenticationProvider>();
builder.Services.AddTransient<RecordLoadingService>();
builder.Services.AddScoped<AuthorizedHttpClientService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
