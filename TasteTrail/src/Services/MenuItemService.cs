#pragma warning disable CS8613
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;
using TasteTrailBlazor.Services.Base;

namespace TasteTrailBlazor.Services;

public class MenuItemService : IMenuItemService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuItemService(
        ILocalStorageService localStorageService,
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
            Console.WriteLine(
                $"Error loading menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<MenuItemDto?> GetFilteredMenuItemsAsync(
        FilterType type,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = (int)type,
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync($"/api/MenuItem/GetFiltered", filterRequest);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MenuItemDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<MenuItemDto?> GetFilteredMenuItemsAsync(
        FilterType type,
        int menuId,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = (int)type,
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync(
            $"/api/Menu/MenuItem?menuId={menuId}",
            filterRequest
        );
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MenuItemDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<bool> CreateMenuItemAsync(MenuItemCreateDto menuItemDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/MenuItem/Create", menuItemDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error creating menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMenuItemAsync(MenuItemDto menuItemDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/MenuItem/Update", menuItemDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error updating menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/MenuItem/DeleteById?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error deleting menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }
}
