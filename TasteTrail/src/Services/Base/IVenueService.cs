using TasteTrailBlazor.Dtos;
using Microsoft.AspNetCore.Http;

namespace TasteTrailBlazor.Services.Base;

public interface IVenueService
{
    Task<List<VenueDto>?> GetVenuesAsync(int from, int to);
    Task<VenueDto> GetVenueByIdAsync(int id);
    Task<int> GetVenueCountAsync();
    Task<bool> CreateVenueAsync(VenueDto venueDto);
    Task<bool> DeleteVenueByIdAsync(int id);
    Task<bool> UpdateVenueAsync(VenueDto venueDto);
    Task<bool> UploadVenueLogoAsync(int venueId, IFormFile logo);
}
