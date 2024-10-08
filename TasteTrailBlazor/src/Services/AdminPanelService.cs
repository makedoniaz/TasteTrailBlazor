#pragma warning disable CS8613
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;
using TasteTrailBlazor.Services.Base;

namespace TasteTrailBlazor.Services;

public class AdminPanelService : IAdminPanelService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminPanelService(
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
        var client = _httpClientFactory.CreateClient("AuthPolicy");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }

        return client;
    }

    public async Task<int> GetUsersCountAsync(
        FilterType type = FilterType.NoFilter,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var client = await CreateAuthenticatedClientAsync();

        var filterRequest = new
        {
            type = type != FilterType.NoFilter ? (int?)type - 1 : null,
            pageNumber = pageNumber,
            pageSize = pageSize,
        };

        var response = await client.PostAsJsonAsync("/api/AdminPanel/User/Count", filterRequest);

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<UsersCountResponse>())!.UsersCount;
        }

        return 0;
    }

    public async Task<bool> AssignRoleAsync(string userId, UserRoles role)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsync(
            $"/api/AdminPanel/AssignRole?userId={userId}&roleId={(int)role}",
            null
        );
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRoleAsync(string userId, UserRoles role)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsync(
            $"/api/AdminPanel/RemoveRole?userId={userId}&roleId={(int)role}",
            null
        );
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ToggleMuteAsync(string userId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsync($"/api/AdminPanel/ToggleMute/{userId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ToggleBanAsync(string userId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsync($"/api/AdminPanel/ToggleBan/{userId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var client = await CreateAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<UserDto>($"/api/AdminPanel/UserInfo/{userId}");
    }

    public async Task<UserListDto?> GetAllUsersAsync(
        FilterType type,
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

        var response = await client.PostAsJsonAsync("/api/AdminPanel/UserFilter", filterRequest);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserListDto>();
        }

        return null;
    }
}
