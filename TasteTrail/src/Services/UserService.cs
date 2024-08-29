using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Services.Base;

namespace TasteTrailBlazor.Services;

public class UserService : IUserService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(
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

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var client = await CreateAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<UserDto>($"/api/User/{userId}");
    }

    public async Task<bool> UpdateUserAsync(string userId, UserDto userDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync($"/api/User/{userId}", userDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<UserDto?> GetUserRolesAsync(string userId)
    {
        var client = await CreateAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<UserDto>($"/api/User/GetUserRoles");
    }
}
