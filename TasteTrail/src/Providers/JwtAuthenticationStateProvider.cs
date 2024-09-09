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

// public class JwtAuthenticationStateProvider : AuthenticationStateProvider
// {
//     private readonly ILocalStorageService _localStorageService;
//     private readonly IHttpClientFactory _httpClientFactory;
//     private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
//     private readonly NavigationManager _navigationManager;
//     private readonly IConfiguration _configuration;

//     public JwtAuthenticationStateProvider(
//         ILocalStorageService localStorageService,
//         IHttpClientFactory httpClientFactory,
//         NavigationManager navigationManager,
//         IConfiguration configuration
//     )
//     {
//         this._localStorageService = localStorageService;
//         this._httpClientFactory = httpClientFactory;
//         this._jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
//         this._navigationManager = navigationManager;
//         this._configuration = configuration;
//     }

//     private async Task<List<Claim>?> GetClaimsIdentity()
//     {
//         var emptyResult = new List<Claim>();
//         string accessToken;
//         string refreshToken;
//         try
//         {
//             accessToken = (await this._localStorageService.GetItemAsStringAsync("jwt"))!;
//             refreshToken = (await this._localStorageService.GetItemAsStringAsync("refresh"))!;

//             Console.WriteLine($"Old JWT: {accessToken}");
//             Console.WriteLine($"Old Refresh Token: {refreshToken}");
//         }
//         catch
//         {
//             this._navigationManager.NavigateTo("/Login");
//             await NotifyUserLogout();
//             return emptyResult;
//         }

//         if (string.IsNullOrEmpty(accessToken))
//             return emptyResult;

//         // Валидация и декодирование токена
//         var claims = JwtTokenHelper.ValidateDecodeToken(accessToken, _configuration);

//         Console.WriteLine(claims.Count);
//         if (claims.Count != 0)
//             return claims;

//         // Если access token просрочен, проверяем наличие refresh token
//         if (!string.IsNullOrEmpty(refreshToken))
//         {
//             var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
//             httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
//                 "Bearer",
//                 accessToken
//             );

//             var response = await httpClient.PutAsJsonAsync(
//                 "/api/Authentication/UpdateToken",
//                 Guid.Parse(refreshToken)
//             );

//             if (response.IsSuccessStatusCode!)
//             {
//                 var accessTokenResult = await response.Content.ReadFromJsonAsync<AccessToken>();

//                 if (accessTokenResult == null)
//                 {
//                     Console.WriteLine("Failed to read access token from response.");
//                     return emptyResult;
//                 }

//                 Console.WriteLine($"New JWT: {accessTokenResult!.Jwt}");
//                 Console.WriteLine($"New Refresh Token: {accessTokenResult!.Refresh}");

//                 await this._localStorageService.SetItemAsStringAsync("jwt", accessTokenResult.Jwt!);
//                 await this._localStorageService.SetItemAsStringAsync(
//                     "refresh",
//                     accessTokenResult.Refresh.ToString()
//                 );

//                 claims = JwtTokenHelper.ValidateDecodeToken(accessTokenResult.Jwt!, _configuration);
//                 return claims;
//             }
//             else
//             {
//                 await NotifyUserLogout();
//                 //this._navigationManager.NavigateTo("/Login");
//             }
//         }
//         else
//         {
//             await NotifyUserLogout();
//             //this._navigationManager.NavigateTo("/Login");
//         }
//         return claims;
//     }

//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         var claims = await GetClaimsIdentity();
//         var claimsIdentity =
//             claims!.Count != 0 ? new ClaimsIdentity(claims, "jwt") : new ClaimsIdentity();
//         var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//         return new AuthenticationState(claimsPrincipal);
//     }

//     public async Task LoginAsync(LoginDto loginDto)
//     {
//         var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
//         var response = await httpClient.PostAsJsonAsync("/api/Authentication/Login", loginDto);

//         if (response.IsSuccessStatusCode)
//         {
//             var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();
//             await this._localStorageService.SetItemAsStringAsync("jwt", accessToken!.Jwt!);
//             await this._localStorageService.SetItemAsStringAsync(
//                 "refresh",
//                 accessToken.Refresh.ToString()
//             );

//             base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//             this._navigationManager.NavigateTo("/");
//         }
//         else
//         {
//             throw new Exception("Login failed. Please check your credentials.");
//         }
//     }

//     public async Task RegisterAsync(RegistrationDto registrationDto)
//     {
//         var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
//         var response = await httpClient.PostAsJsonAsync(
//             "/api/Authentication/Registration",
//             registrationDto
//         );

//         if (response.IsSuccessStatusCode)
//         {
//             this._navigationManager.NavigateTo("/Login");
//         }
//         else
//         {
//             throw new Exception("Registration failed. Please try again.");
//         }
//     }

//     public async Task LogoutAsync()
//     {
//         var token = await this._localStorageService.GetItemAsStringAsync("jwt");
//         var refreshToken = await this._localStorageService.GetItemAsStringAsync("refresh");
//         var httpClient = this._httpClientFactory.CreateClient("AuthPolicy");
//         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
//             "Bearer",
//             token
//         );
//         var response = await httpClient.PatchAsJsonAsync(
//             "/api/Authentication/Logout",
//             refreshToken
//         );

//         if (response.IsSuccessStatusCode)
//         {
//             await NotifyUserLogout();
//             this._navigationManager.NavigateTo("/Login");
//         }
//     }

//     public async Task NotifyUserLogout()
//     {
//         var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//         var authState = Task.FromResult(new AuthenticationState(anonymousUser));
//         NotifyAuthenticationStateChanged(authState);

//         // await this._localStorageService.RemoveItemAsync("jwt");
//         // await this._localStorageService.RemoveItemAsync("refresh");
//     }
// }
