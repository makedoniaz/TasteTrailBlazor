#pragma warning disable CS8613

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Blazored.LocalStorage;
 using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TasteTrailBlazor.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly LoginService _loginService;

    public JwtAuthenticationStateProvider(LoginService loginService)
    {
        _loginService = loginService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claims = await _loginService.GetLoginInfoAsync();
        var claimsIdentity =
            claims.Count != 0
                ? new ClaimsIdentity(claims, "jwt") 
                : new ClaimsIdentity();
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return new AuthenticationState(claimsPrincipal);
    }
}
