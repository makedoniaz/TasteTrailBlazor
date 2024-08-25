using System.Net.Http.Json;
using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Services.Base;
using Blazored.LocalStorage; 
using System.Net.Http.Headers;
using TasteTrailBlazor.Models; 

namespace TasteTrailBlazor.Services;

public class FeedbackService : IFeedbackService
{
 private readonly ILocalStorageService _localStorageService;
    private readonly IHttpClientFactory _httpClientFactory;
    public FeedbackService(ILocalStorageService localStorageService,
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
            Console.WriteLine($"Error loading feedback: {response.StatusCode} - {response.ReasonPhrase}");
            return null;
        }
    }

    public async Task<List<Feedback>?> GetFeedbacksByCountAsync(int count)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Feedback/GetByCount?count={count}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Feedback>>();
        }
        else
        {
            Console.WriteLine($"Error loading feedbacks: {response.StatusCode} - {response.ReasonPhrase}");
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
            Console.WriteLine($"Error getting feedback count: {response.StatusCode} - {response.ReasonPhrase}");
            return 0;
        }
    }

    public async Task CreateFeedbackAsync(FeedbackDto feedbackDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Feedback/Create", feedbackDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error creating feedback: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    public async Task UpdateFeedbackAsync(FeedbackDto feedbackDto)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Feedback/Update", feedbackDto);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error updating feedback: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    public async Task DeleteFeedbackAsync(int id)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Feedback/DeleteById/{id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error deleting feedback: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }
}
