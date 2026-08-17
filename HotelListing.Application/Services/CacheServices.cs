using HotelListing.Application.DTOs.Country;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Results;
using Microsoft.Extensions.Caching.Memory;

namespace HotelListing.Application.Services;

public class CacheServices(IMemoryCache cache) : ICacheServices
{
    public void InvalidateCountryCache(int id)
    {
        cache.Remove($"{CacheNames.CountryCache}{id}");
    }

    public Result<IEnumerable<GetCountriesDto>> GetCountriesFromCache(string? options)
    {
        var cacheKey = $"{CacheNames.CountriesListCache}{options ?? string.Empty}";
        return cache.TryGetValue(cacheKey, out IEnumerable<GetCountriesDto>? countries)
               && countries is not null
            ? Result<IEnumerable<GetCountriesDto>>.Success(countries)
            : Result<IEnumerable<GetCountriesDto>>.NotFound();
    }


    public Result<GetCountryDto> GetCountryFromCacheById(int id)
    {
        var cacheKey = $"{CacheNames.CountryCache}{id}";
        return cache.TryGetValue(cacheKey, out GetCountryDto? country)
               && country is not null
            ? Result<GetCountryDto>.Success(country)
            : Result<GetCountryDto>.NotFound();
    }

    public void AddCountryToCacheById(GetCountryDto country)
    {
        var cacheOptions = CacheOptions.CountryCacheOptions;
        var cacheKey = $"{CacheNames.CountryCache}{country.Id}";
        cache.Set(cacheKey, country, cacheOptions);
    }
}