#pragma warning disable CS8613
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;
using TasteTrailBlazor.Services.Base;

namespace TasteTrailBlazor.Services;

public class MenuService : IMenuService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuService(
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

    public async Task<MenuDto?> GetFilteredMenusAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync($"/api/Menu/GetFiltered", filterRequest);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MenuDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<MenuDto?> GetFilteredMenusAsync(
        int venueId,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync(
            $"/api/Menu/GetFiltered?venueId={venueId}",
            filterRequest
        );
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MenuDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<Menu?> GetMenuByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Menu/GetById?id={id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Menu>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<List<Menu>?> GetMenusFromToAsync(int from, int to, int venueId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync(
            $"/api/Menu/GetFromTo?from={from}&to={to}&venueId={venueId}"
        );
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Menu>>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading menus: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<bool> CreateMenuAsync(MenuCreateDto menuDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Menu/Create", menuDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error creating menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMenuAsync(MenuDto menuDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Menu/Update", menuDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error updating menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Menu/DeleteById?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error deleting menu: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }
}
