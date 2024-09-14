#pragma warning disable CS8613
#pragma warning disable CS8604
#pragma warning disable CS8600
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Providers;

public class LoginService
{
    private const string AccessToken = nameof(AccessToken);
    private const string RefreshToken = nameof(RefreshToken);

    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginService(
        ILocalStorageService localStorage,
        NavigationManager navigation,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration
    )
    {
        this._localStorage = localStorage;
        this._navigation = navigation;
        this._configuration = configuration;
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
            var response = await httpClient.PostAsJsonAsync("/api/Authentication/Login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

                await this._localStorage.SetItemAsStringAsync(AccessToken, accessToken!.Jwt!);
                await this._localStorage.SetItemAsStringAsync(
                    RefreshToken,
                    accessToken!.Refresh!.ToString()
                );

                this._navigation.NavigateTo("/");
                return null;
            }
            else
            {
                return "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> RegisterAsync(RegistrationDto registrationDto)
    {
        try
        {
            var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
            var response = await httpClient.PostAsJsonAsync(
                "/api/Authentication/Registration",
                registrationDto
            );

            if (response.IsSuccessStatusCode)
            {
                this._navigation.NavigateTo("/Login");
                return null;
            }
            else
            {
                return "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<List<Claim>> GetLoginInfoAsync()
    {
        var emptyResult = new List<Claim>();
        string accessToken;
        string refreshToken;
        try
        {
            accessToken = await this._localStorage.GetItemAsStringAsync(AccessToken);
            refreshToken = await this._localStorage.GetItemAsStringAsync(RefreshToken);
        }
        catch
        {
            await LogoutAsync();
            return emptyResult;
        }

        if (string.IsNullOrEmpty(accessToken))
            return emptyResult;

        var claims = JwtTokenHelper.ValidateDecodeToken(accessToken, this._configuration);

        if (claims.Count != 0)
            return claims;

        if (!string.IsNullOrEmpty(refreshToken!.ToString()))
        {
            var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );

            var response = await httpClient.PutAsJsonAsync(
                "/api/Authentication/UpdateToken",
                refreshToken
            );

            if (response.IsSuccessStatusCode!)
            {
                var accessTokenResult = await response.Content.ReadFromJsonAsync<AccessToken>();

                if (accessTokenResult == null)
                {
                    Console.WriteLine("Failed to read access token from response.");
                    await LogoutAsync();
                    return emptyResult;
                }

                await _localStorage.SetItemAsStringAsync(AccessToken, accessTokenResult!.Jwt!);
                await _localStorage.SetItemAsStringAsync(
                    RefreshToken,
                    accessTokenResult.Refresh.ToString()
                );
                claims = JwtTokenHelper.ValidateDecodeToken(
                    accessTokenResult.Jwt,
                    this._configuration
                );
                return claims;
            }
            else
            {
                await LogoutAsync();
                return emptyResult;
            }
        }
        else
        {
            await LogoutAsync();
            return emptyResult;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var token = await this._localStorage.GetItemAsStringAsync(AccessToken);
            var refreshToken = await this._localStorage.GetItemAsStringAsync(RefreshToken);
            var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
            var response = await httpClient.PatchAsJsonAsync(
                "/api/Authentication/Logout",
                refreshToken
            );
        }
        catch { }
        finally
        {
            await RemoveAuthDataFromStorageAsync();
        }
    }

    private async Task RemoveAuthDataFromStorageAsync()
    {
        await this._localStorage.RemoveItemAsync(AccessToken);
        await this._localStorage.RemoveItemAsync(RefreshToken);
    }
}
