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
        var token = await _localStorageService.GetItemAsStringAsync("AccessToken");
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
        FilterType type = FilterType.NoFilter,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = type != FilterType.NoFilter ? (int?)type - 1 - 1 : null,
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
        int menuId,
        FilterType type = FilterType.NoFilter,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = type != FilterType.NoFilter ? (int?)type - 1 : null,
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

    public async Task<bool> CreateMenuItemAsync(
        MenuItemCreateDto menuItemDto,
        Stream imageStream,
        string imageFileName
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        using var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(menuItemDto.Name ?? string.Empty), nameof(menuItemDto.Name));
        formData.Add(
            new StringContent(menuItemDto.Description ?? string.Empty),
            nameof(menuItemDto.Description)
        );
        formData.Add(new StringContent(menuItemDto.Price.ToString()), nameof(menuItemDto.Price));
        formData.Add(new StringContent(menuItemDto.MenuId.ToString()), nameof(menuItemDto.MenuId));
        
        Console.WriteLine(menuItemDto.Name);
        Console.WriteLine(menuItemDto.Description);
        Console.WriteLine(menuItemDto.MenuId);
        Console.WriteLine(imageStream);
            
        if (imageStream != null)
        {
            var fileContent = new StreamContent(imageStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "logo", imageFileName);
        }

        var response = await client.PostAsync("/api/MenuItem/Create", formData);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error creating menu item: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMenuItemAsync(
        MenuItemUpdateDto menuItemDto,
        Stream imageStream,
        string imageFileName
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        using var formData = new MultipartFormDataContent();

        formData.Add(
            new StringContent(menuItemDto.Id.ToString() ?? string.Empty),
            nameof(menuItemDto.Id)
        );
        formData.Add(new StringContent(menuItemDto.Name ?? string.Empty), nameof(menuItemDto.Name));
        formData.Add(
            new StringContent(menuItemDto.Description ?? string.Empty),
            nameof(menuItemDto.Description)
        );
        formData.Add(new StringContent(menuItemDto.Price.ToString()), nameof(menuItemDto.Price));

        if (imageStream != null)
        {
            var fileContent = new StreamContent(imageStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "logo", imageFileName);
        }

        var response = await client.PutAsync("/api/MenuItem/Update", formData);
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
