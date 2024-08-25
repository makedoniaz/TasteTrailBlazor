using System.Net.Http.Json;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Services.Base;
using Microsoft.AspNetCore.Http; 
using Blazored.LocalStorage; 
using System.Net.Http.Headers;
namespace TasteTrailBlazor.Services;

public class VenueService : IVenueService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public VenueService(
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

    public async Task<List<VenueDto>?> GetVenuesAsync(int from, int to)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Venue/GetFromTo?from={from}&to={to}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<VenueDto>>();
        }

        return new List<VenueDto>();
    }

    public async Task<VenueDto> GetVenueByIdAsync(Guid id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Venue/GetById?id={id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VenueDto>();
        }

        return null;
    }

    public async Task<int> GetVenueCountAsync()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Venue/GetCount");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<int>();
        }

        return 0;
    }

    public async Task<bool> CreateVenueAsync(VenueDto venueDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Venue/Create", venueDto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteVenueByIdAsync(Guid id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Venue/DeleteById?id={id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateVenueAsync(VenueDto venueDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Venue/Update", venueDto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UploadVenueLogoAsync(Guid venueId, IFormFile logo)
    {
        var client = await CreateAuthenticatedClientAsync();
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(logo.OpenReadStream()), "file", logo.FileName);
        var response = await client.PostAsync($"/api/VenueLogo/Create?venueId={venueId}", content);

        return response.IsSuccessStatusCode;
    }
}
