using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;
using TasteTrailBlazor.Services.Base;

namespace TasteTrailBlazor.Services;

public class FeedbackService : IFeedbackService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public FeedbackService(
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

    public async Task<Feedback?> GetFeedbackByIdAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Feedback/GetById/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Feedback>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<FeedbackDto?> GetFilteredFeedbacksAsync(
        FilterType type,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = type,
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync($"/api/Feedback/GetFiltered", filterRequest);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<FeedbackDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<FeedbackDto?> GetFilteredFeedbacksAsync(
        FilterType type,
        int venueId,
        int pageNumber = 1,
        int pageSize = 10,
        string searchTerm = ""
    )
    {
        var client = await CreateAuthenticatedClientAsync();
        var filterRequest = new
        {
            type = type,
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchTerm = searchTerm,
        };
        var response = await client.PostAsJsonAsync(
            $"/api/Feedback/GetFiltered?venueId={venueId}",
            filterRequest
        );
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<FeedbackDto>();
        }
        else
        {
            Console.WriteLine(
                $"Error loading feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return null;
        }
    }

    public async Task<int> GetFeedbackCountAsync()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/Feedback/GetCount");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<int>();
        }
        else
        {
            Console.WriteLine(
                $"Error getting feedback count: {response.StatusCode} - {response.ReasonPhrase}"
            );
            return 0;
        }
    }

    public async Task<bool> CreateFeedbackAsync(FeedbackDto feedbackDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Feedback/Create", feedbackDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error creating feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateFeedbackAsync(FeedbackDto feedbackDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Feedback/Update", feedbackDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error updating feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFeedbackAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Feedback/DeleteById/{id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Error deleting feedback: {response.StatusCode} - {response.ReasonPhrase}"
            );
        }
        return response.IsSuccessStatusCode;
    }
}
