using HotelListing.Application.DTOs.Country;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface ICacheServices
{
    void InvalidateCountryCache(int id);
    Result<GetCountryDto> GetCountryFromCacheById(int id);
    void AddCountryToCacheById(GetCountryDto country);
}