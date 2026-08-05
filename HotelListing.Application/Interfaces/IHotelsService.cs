using HotelListing.Application.DTOs.Hotel;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface IHotelsService
{
    Task<Result<PagedResult<GetHotelsDto>>> GetHotelsAsync(HotelFilterParameters filters,PaginationParameters paginationParameters);
    Task<Result<GetHotelDto>> GetHotelAsync(int id);
    Task<Result<GetHotelDto>> UpdateHotelAsync(int id,UpdateHotelDto updateHotelDto);
    Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto);
    Task<Result> DeleteHotelAsync(int id);
}