using TasteTrailBlazor.Dtos;
using Microsoft.AspNetCore.Http;
using TasteTrailBlazor.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace TasteTrailBlazor.Services.Base;

public interface IVenueService
{
    Task<VenueDto?> GetFilteredVenuesAsync(FilterType type = FilterType.NoFilter, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<Venue> GetVenueByIdAsync(int id);
    Task<int> GetVenueCountAsync();
    Task<bool> CreateVenueAsync(VenueCreateDto venue, Stream logoStream, string logoFileName);
    Task<bool> DeleteVenueByIdAsync(int id);
    Task<bool> UpdateVenueAsync(Venue venueDto);
    Task<bool> UploadVenueLogoAsync(int venueId, IBrowserFile logo);
}
