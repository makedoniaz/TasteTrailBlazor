using TasteTrailBlazor.Dtos;
using Microsoft.AspNetCore.Http;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IVenueService
{
    Task<VenueDto?> GetFilteredVenuesAsync(FilterType type, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<VenueDto> GetVenueByIdAsync(int id);
    Task<int> GetVenueCountAsync();
    Task<bool> CreateVenueAsync(VenueDto venueDto);
    Task<bool> DeleteVenueByIdAsync(int id);
    Task<bool> UpdateVenueAsync(VenueDto venueDto);
    Task<bool> UploadVenueLogoAsync(int venueId, IFormFile logo);
}
