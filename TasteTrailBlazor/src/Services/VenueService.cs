#pragma warning disable CS8613
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
        var token = await this._localStorageService.GetItemAsStringAsync("AccessToken");
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

    public async Task<VenueDto?> GetFilteredVenuesAsync(FilterType type = FilterType.NoFilter)
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
            type = type != FilterType.NoFilter ? (int?)type - 1 : null,
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

    public async Task<Venue?> GetVenueByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Venue/GetById?id={id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Venue>();
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

    public async Task<bool> CreateVenueAsync(
        VenueCreateDto venue,
        Stream logoStream,
        string logoFileName
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        using var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(venue.Name ?? string.Empty), nameof(venue.Name));
        formData.Add(new StringContent(venue.Description ?? string.Empty), nameof(venue.Description));
        formData.Add(new StringContent(venue.Address ?? string.Empty), nameof(venue.Address));
        formData.Add(new StringContent(venue.Email ?? string.Empty), nameof(venue.Email));
        formData.Add(new StringContent(venue.ContactNumber ?? string.Empty), nameof(venue.ContactNumber));
        formData.Add(new StringContent(venue.AveragePrice.ToString() ?? string.Empty), nameof(venue.AveragePrice));
        formData.Add(new StringContent(venue.Longtitude.ToString() ?? string.Empty), nameof(venue.Longtitude));
        formData.Add(new StringContent(venue.Latitude.ToString() ?? string.Empty), nameof(venue.Latitude));

        if (logoStream != null ) 
        {
            var fileContent = new StreamContent(logoStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");  
            formData.Add(fileContent, "logo", logoFileName);  
        }
        Console.WriteLine($"Server responded with error: {logoStream}");

        var response = await client.PostAsync("/api/Venue/Create", formData);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Server responded with error: {responseContent}");
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteVenueByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Venue/DeleteById?id={id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateVenueAsync(VenueUpdateDto venue,  Stream logoStream, string logoFileName)
    {
         var client = await CreateAuthenticatedClientAsync();
        using var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(venue.Id.ToString() ?? string.Empty), nameof(venue.Id));
        formData.Add(new StringContent(venue.Name ?? string.Empty), nameof(venue.Name));
        formData.Add(new StringContent(venue.Description ?? string.Empty), nameof(venue.Description));
        formData.Add(new StringContent(venue.Address ?? string.Empty), nameof(venue.Address));
        formData.Add(new StringContent(venue.Email ?? string.Empty), nameof(venue.Email));
        formData.Add(new StringContent(venue.ContactNumber ?? string.Empty), nameof(venue.ContactNumber));
        formData.Add(new StringContent(venue.AveragePrice.ToString() ?? string.Empty), nameof(venue.AveragePrice));
        formData.Add(new StringContent(venue.Longtitude.ToString() ?? string.Empty), nameof(venue.Longtitude));
        formData.Add(new StringContent(venue.Latitude.ToString() ?? string.Empty), nameof(venue.Latitude));
        
        if (logoStream != null ) 
        {
            var fileContent = new StreamContent(logoStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");  
            formData.Add(fileContent, "logo", logoFileName);  
        }
        Console.WriteLine($"Server responded with error: {logoStream}");

        var response = await client.PutAsync("/api/Venue/Update", formData);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Server responded with error: {responseContent}");
        }

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
