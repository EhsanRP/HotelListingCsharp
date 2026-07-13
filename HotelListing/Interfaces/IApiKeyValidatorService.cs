namespace HotelListing.Interfaces;

public interface IApiKeyValidatorService
{
    Task<bool> IsValidAsync(string apiKey, CancellationToken cancellationToken = default);
}