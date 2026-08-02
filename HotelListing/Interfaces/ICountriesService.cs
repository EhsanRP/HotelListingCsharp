using HotelListing.Common.Results;
using HotelListing.DTOs.Country;

namespace HotelListing.Interfaces;

public interface ICountriesService
{
    Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result<GetCountryDto>> UpdateCountryAsync(int id, UpdateCountryDto countryDto);
    Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
    Task<Result> DeleteCountryAsync(int id);
}