using HotelListing.Application.DTOs.Country;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface ICountriesService
{
    Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(CountryFilterParameters filters,PaginationParameters paginationParameters);
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result<GetCountryDto>> UpdateCountryAsync(int id, UpdateCountryDto countryDto);
    Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
    Task<Result> DeleteCountryAsync(int id);
}