using System.Net.Http.Json;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Services.Base;
using Blazored.LocalStorage; 
using System.Net.Http.Headers;
using TasteTrailBlazor.Models; 
namespace TasteTrailBlazor.Services.Base;

public class MenuItemService : IMenuItemService
{
 private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;
    public MenuItemService(ILocalStorageService localStorageService,
        IHttpClientFactory httpClientFactory
    )
    {
        _localStorageService = localStorageService;
        _httpClientFactory = httpClientFactory;
    } 
    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await _localStorageService.GetItemAsStringAsync("jwt");
        var client = _httpClientFactory.CreateClient("ExperincePolicy");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }

        return client;
    }
    public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/MenuItem/GetById?id={id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MenuItem>();
        }
        else
        {
            Console.WriteLine($"Error loading menu item: {response.StatusCode} - {response.ReasonPhrase}");
            return null;
        }
    }

    public async Task<List<MenuItem>?> GetMenuItemsFromToAsync(int from, int to, int menuId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/MenuItem/GetFromTo?from={from}&to={to}&menuId={menuId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<MenuItem>>();
        }
        else
        {
            Console.WriteLine($"Error loading menu items: {response.StatusCode} - {response.ReasonPhrase}");
            return null;
        }
    }

    public async Task CreateMenuItemAsync(MenuItemDto menuItemDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/MenuItem/Create", menuItemDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error creating menu item: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    public async Task UpdateMenuItemAsync(MenuItemDto menuItemDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/MenuItem/Update", menuItemDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error updating menu item: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/MenuItem/DeleteById?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error deleting menu item: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }
}
