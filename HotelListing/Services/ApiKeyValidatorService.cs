using HotelListing.Data;
using HotelListing.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Services;

public class ApiKeyValidatorService(HotelListingDbContext context) : IApiKeyValidatorService
{
    public async Task<bool> IsValidAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }

        var apiKeyEntity = await context.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Key == apiKey, cancellationToken);

        return apiKeyEntity is not null && apiKeyEntity.IsActive;
    }
}