using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using TasteTrailBlazor.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace TasteTrailBlazor.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorageService,
        IHttpClientFactory httpClientFactory)
    {
        this.localStorageService = localStorageService;
        this.httpClientFactory = httpClientFactory;
        this.jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

private async Task<ClaimsIdentity?> GetClaimsIdentity()
{
    var jwt = await localStorageService.GetItemAsStringAsync("jwt");
    if (string.IsNullOrWhiteSpace(jwt))
        return null;

    var result = await jwtSecurityTokenHandler.ValidateTokenAsync(jwt, new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "TasteTrailIdentity",

        ValidateAudience = true,
        ValidAudience = "Wolf-Street-Developers",

        SignatureValidator = (token, validationParameters) => new JwtSecurityToken(token),

        RequireExpirationTime = true,
        ValidateLifetime = true,

        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires > DateTime.UtcNow,
    });

    if (!result.IsValid && result.Exception is SecurityTokenInvalidLifetimeException)
    {
        var success = await TryRefreshTokenAsync();
        if (!success)
            return null;

         jwt = await localStorageService.GetItemAsStringAsync("jwt");
        var newTokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);
        return new ClaimsIdentity(newTokenObj.Claims, "jwt");
    }

    var tokenObj = jwtSecurityTokenHandler.ReadJwtToken(jwt);
    return new ClaimsIdentity(tokenObj.Claims, "jwt");
}

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await GetClaimsIdentity();

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

public void NotifyUserLogout()
{
    var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
    var authState = Task.FromResult(new AuthenticationState(anonymousUser));
    NotifyAuthenticationStateChanged(authState);
}

}