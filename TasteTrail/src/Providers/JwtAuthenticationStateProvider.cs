using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;
    private readonly NavigationManager navigationManager;

    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorageService,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager)
    {
        this.localStorageService = localStorageService;
        this.httpClientFactory = httpClientFactory;
        this.jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        this.navigationManager = navigationManager;
    }

    private async Task<ClaimsIdentity?> GetClaimsIdentity()
    {
        var jwt = await localStorageService.GetItemAsStringAsync("jwt");
        if (string.IsNullOrWhiteSpace(jwt))
            return null;

        try
        {
            var result = await jwtSecurityTokenHandler.ValidateTokenAsync(jwt, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "TasteTrailIdentity",
                ValidateAudience = true,
                ValidAudience = "Wolf-Street-Developers",
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("Your_Secret_Key_Here")),
                ClockSkew = TimeSpan.Zero
            });

            var tokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);
            return new ClaimsIdentity(tokenObj.Claims, "jwt");
        }
        catch (SecurityTokenExpiredException)
        {
            var success = await TryRefreshTokenAsync();
            if (!success)
            {
                return null;
            }

            jwt = await localStorageService.GetItemAsStringAsync("jwt");
            var newTokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);
            return new ClaimsIdentity(newTokenObj.Claims, "jwt");
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await GetClaimsIdentity();

        if (claimsIdentity == null)
        {
            var result = await TryRefreshTokenAsync();
            if (!result)
            {
                // this.navigationManager.NavigateTo("/");
                // await NotifyUserLogout();
            }
        }

        var claimsPrincipal = claimsIdentity == null
            ? new ClaimsPrincipal(new ClaimsIdentity())
            : new ClaimsPrincipal(claimsIdentity);

        return new AuthenticationState(claimsPrincipal);
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        var refreshToken = await localStorageService.GetItemAsStringAsync("refresh");

        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var httpClient = httpClientFactory.CreateClient("IdentityService");
        var response = await httpClient.PutAsJsonAsync("/api/Authentication/UpdateToken", new { RefreshToken = refreshToken });

        if (!response.IsSuccessStatusCode)
            return false;

        var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();

        if (accessToken == null)
            return false;

        await localStorageService.SetItemAsStringAsync("jwt", accessToken.Jwt);
        await localStorageService.SetItemAsStringAsync("refresh", accessToken.Refresh.ToString());

        return true;
    }

    public async Task NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);

        await localStorageService.RemoveItemAsync("jwt");
        await localStorageService.RemoveItemAsync("refresh");
    }
}
