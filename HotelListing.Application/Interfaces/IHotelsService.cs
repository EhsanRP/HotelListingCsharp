using HotelListing.Application.DTOs.Hotel;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface IHotelsService
{
    Task<Result<IEnumerable<GetHotelsDto>>> GetHotelsAsync();
    Task<Result<GetHotelDto>> GetHotelAsync(int id);
    Task<Result<GetHotelDto>> UpdateHotelAsync(int id,UpdateHotelDto updateHotelDto);
    Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto);
    Task<Result> DeleteHotelAsync(int id);
}