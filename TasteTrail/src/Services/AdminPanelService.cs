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
        var token = await _localStorageService.GetItemAsStringAsync("jwt");
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

    public async Task<int> GetUsersCountAsync()
    {
        var client = await CreateAuthenticatedClientAsync();
        var result = await client.GetFromJsonAsync<UsersCountResponse>(
            "/api/AdminPanel/GetUsersCount"
        );
        return result!.UsersCount;
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
        return await client.GetFromJsonAsync<UserDto>($"/api/AdminPanel/User/{userId}");
    }

    public async Task<List<UserDto>?> GetAllUsersAsync(int from, int to)
    {
        var client = await CreateAuthenticatedClientAsync();
        var users = await client.GetFromJsonAsync<List<UserDto>>(
            $"/api/AdminPanel/User??-from={from}&to={to}"
        );
        return users?.ToList();
    }
}
