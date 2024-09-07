using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IFeedbackService
{
    Task<FeedbackDto?> GetFilteredFeedbacksAsync(FilterType type, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<FeedbackDto?> GetFilteredFeedbacksAsync(FilterType type, int venueId, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
     Task<int> GetFeedbackCountAsync();
    Task<bool> CreateFeedbackAsync(FeedbackDto feedbackDto);
    Task<bool> UpdateFeedbackAsync(FeedbackDto feedbackDto);
    Task<bool> DeleteFeedbackAsync(int id);
}
