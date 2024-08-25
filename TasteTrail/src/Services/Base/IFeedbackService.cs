using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IFeedbackService
{
    Task<Feedback?> GetFeedbackByIdAsync(int id);
    Task<List<Feedback>?> GetFeedbacksByCountAsync(int count);
    Task<int> GetFeedbackCountAsync();
    Task CreateFeedbackAsync(FeedbackDto feedbackDto);
    Task UpdateFeedbackAsync(FeedbackDto feedbackDto);
    Task DeleteFeedbackAsync(int id);
}
