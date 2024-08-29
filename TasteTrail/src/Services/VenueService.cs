using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;
using TasteTrailBlazor.Services.Base;

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
        this._localStorageService = localStorageService;
        this._httpClientFactory = httpClientFactory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await this._localStorageService.GetItemAsStringAsync("jwt");
        var client = this._httpClientFactory.CreateClient("ExperincePolicy");

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }

        return client;
    }

    public async Task<VenueDto?> GetFilteredVenuesAsync(FilterType type)
    {
        return await GetFilteredVenuesAsync(type, 1, 10, "");
    }

    public async Task<VenueDto?> GetFilteredVenuesAsync(
        FilterType type,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();

        var filterRequest = new
        {
            type = (int)type,
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };

        var response = await client.PostAsJsonAsync("/api/Venue/GetFiltered", filterRequest);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VenueDto>();
        }

        return null;
    }

    public async Task<VenueDto> GetVenueByIdAsync(int id)
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

    public async Task<bool> CreateVenueAsync(Venue venueDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Venue/Create", venueDto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteVenueByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Venue/DeleteById?id={id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateVenueAsync(Venue venueDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Venue/Update", venueDto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UploadVenueLogoAsync(int venueId, IBrowserFile logo)
    {
        var client = await CreateAuthenticatedClientAsync();
        using var content = new MultipartFormDataContent();
        var stream = logo.OpenReadStream(maxAllowedSize: 1024 * 1024 * 15);
        content.Add(new StreamContent(stream), "file", logo.Name); 
        var response = await client.PostAsync($"/api/VenueLogo/Create?venueId={venueId}", content);

        return response.IsSuccessStatusCode;
    }
}
