using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TasteTrailBlazor;
using TasteTrailBlazor.Providers;
using TasteTrailBlazor.Services;
using TasteTrailBlazor.Services.Base;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Accessing configuration
var authBaseUrl = builder.Configuration["AuthBaseUrl"] ?? "http://localhost:5172/";
var experinceBaseUrl = builder.Configuration["ExperinceBaseUrl"] ?? "http://localhost:5188/";

// Registering services
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(); 
builder.Services.AddAuthorizationCore(options =>  {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// HttpClient registration using configuration settings
builder.Services.AddHttpClient("AuthPolicy", httpClient =>
{
    httpClient.BaseAddress = new Uri(authBaseUrl);
});
builder.Services.AddHttpClient("ExperincePolicy", httpClient =>
{
    httpClient.BaseAddress = new Uri(experinceBaseUrl);
});


builder.Services.AddScoped<IAdminPanelService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new AdminPanelService(localStorageService, httpClientFactory);
});
builder.Services.AddScoped<IUserService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new UserService(localStorageService, httpClientFactory);
});
builder.Services.AddScoped<IVenueService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new VenueService(localStorageService, httpClientFactory);
});
builder.Services.AddScoped<IMenuService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new MenuService(localStorageService, httpClientFactory);
});
builder.Services.AddScoped<IMenuItemService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new MenuItemService(localStorageService, httpClientFactory);
});
builder.Services.AddScoped<IFeedbackService>(provider =>
{
    var localStorageService = provider.GetRequiredService<ILocalStorageService>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new FeedbackService(localStorageService, httpClientFactory);
});

builder.Services.AddBlazoredLocalStorageAsSingleton();

await builder.Build().RunAsync();
