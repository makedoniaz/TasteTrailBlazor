using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TasteTrailBlazor;
using TasteTrailBlazor.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore(options =>  {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddHttpClient("BlazorPolicy", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5172/");
});
builder.Services.AddHttpClient("LocalHostPolicy", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5188/");
});
builder.Services.AddBlazoredLocalStorageAsSingleton();

await builder.Build().RunAsync();
