using HotelListing.Interfaces;

namespace HotelListing.Services;

public class ApiKeyValidatorService(IConfiguration configuration) : IApiKeyValidatorService
{
    public Task<bool> IsValidAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(apiKey.Equals(configuration["ApiKey"]));
    }
}