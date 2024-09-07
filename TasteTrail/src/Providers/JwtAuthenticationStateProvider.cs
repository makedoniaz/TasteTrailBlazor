using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly NavigationManager _navigationManager;
    private readonly IConfiguration _configuration;

    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorageService,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager,
        IConfiguration configuration
    )
    {
        this._localStorageService = localStorageService;
        this._httpClientFactory = httpClientFactory;
        this._jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        this._navigationManager = navigationManager;
        this._configuration = configuration;
    }

    private async Task<ClaimsIdentity?> GetClaimsIdentity()
    {
        var jwt = await this._localStorageService.GetItemAsStringAsync("jwt");
        if (string.IsNullOrWhiteSpace(jwt))
            return null;

        var key = this._configuration["Jwt:Key"];
        var issuer = this._configuration["Jwt:Issuer"];
        var audience = this._configuration["Jwt:Audience"];

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(key!)
                ),
                ClockSkew = TimeSpan.Zero,
            };

            var result = this._jwtSecurityTokenHandler.ValidateToken(
                jwt,
                tokenValidationParameters,
                out var validatedToken
            );

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var claims = result.Claims;
                return new ClaimsIdentity(claims, "jwt");
            }

            return null;
        }
        catch (SecurityTokenExpiredException)
        {
            var success = await TryRefreshTokenAsync();
            if (!success)
            {
                return null;
            }

            jwt = await this._localStorageService.GetItemAsStringAsync("jwt");
            if (string.IsNullOrWhiteSpace(jwt))
                return null;

            var result = this._jwtSecurityTokenHandler.ValidateToken(
                jwt,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(key!)
                    ),
                    ClockSkew = TimeSpan.Zero,
                },
                out var validatedToken
            );

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var claims = result.Claims;
                return new ClaimsIdentity(claims, "jwt");
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await GetClaimsIdentity();

        if (claimsIdentity == null)
        {
            var refreshResult = await TryRefreshTokenAsync();
            if (!refreshResult)
            {
                this._navigationManager.NavigateTo("/");
                await NotifyUserLogout();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            claimsIdentity = await GetClaimsIdentity();
        }

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity ?? new ClaimsIdentity());
        return new AuthenticationState(claimsPrincipal);
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        var token = await this._localStorageService.GetItemAsStringAsync("jwt");
        var refreshToken = await this._localStorageService.GetItemAsStringAsync("refresh");

        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Token or refresh token is missing.");
            return false;
        }

        var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );

        var response = await httpClient.PutAsJsonAsync(
            "/api/Authentication/UpdateToken",
            refreshToken
        );

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to refresh token. Status code: {response.StatusCode}");
            return false;
        }

        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        if (accessToken == null)
        {
            Console.WriteLine("Failed to read access token from response.");
            return false;
        }

        Console.WriteLine($"New JWT: {accessToken.Jwt}");
        Console.WriteLine($"New Refresh Token: {accessToken.Refresh}");

        await this._localStorageService.SetItemAsStringAsync("jwt", accessToken.Jwt!);
        await this._localStorageService.SetItemAsStringAsync(
            "refresh",
            accessToken.Refresh.ToString()
        );

        return true;
    }

    public async Task LoginAsync(LoginDto loginDto)
    {
        var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
        var response = await httpClient.PostAsJsonAsync("/api/Authentication/Login", loginDto);

        if (response.IsSuccessStatusCode)
        {
            var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();
            await this._localStorageService.SetItemAsStringAsync("jwt", accessToken.Jwt!);
            await this._localStorageService.SetItemAsStringAsync(
                "refresh",
                accessToken.Refresh.ToString()
            );

            base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            this._navigationManager.NavigateTo("/");
        }
        else
        {
            throw new Exception("Login failed. Please check your credentials.");
        }
    }

    public async Task RegisterAsync(RegistrationDto registrationDto)
    {
        var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
        var response = await httpClient.PostAsJsonAsync(
            "/api/Authentication/Registration",
            registrationDto
        );

        if (response.IsSuccessStatusCode)
        {
            this._navigationManager.NavigateTo("/Login");
        }
        else
        {
            throw new Exception("Registration failed. Please try again.");
        }
    }

    public async Task LogoutAsync()
    {
        var token = await this._localStorageService.GetItemAsStringAsync("jwt");
        var refreshToken = await this._localStorageService.GetItemAsStringAsync("refresh");
        var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
        var response = await httpClient.PatchAsJsonAsync(
            "/api/Authentication/Logout",
            refreshToken
        );

        if (response.IsSuccessStatusCode)
        {
            await NotifyUserLogout();
            this._navigationManager.NavigateTo("/Login");
        }
    }

    public async Task NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);

        await this._localStorageService.RemoveItemAsync("jwt");
        await this._localStorageService.RemoveItemAsync("refresh");
    }
}
