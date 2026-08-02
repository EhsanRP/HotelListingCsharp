using HotelListing.Common.Results;
using HotelListing.DTOs.Hotel;

namespace HotelListing.Interfaces;

public interface IHotelsService
{
    Task<Result<IEnumerable<GetHotelsDto>>> GetHotelsAsync();
    Task<Result<GetHotelDto>> GetHotelAsync(int id);
    Task<Result<GetHotelDto>> UpdateHotelAsync(int id,UpdateHotelDto updateHotelDto);
    Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto);
    Task<Result> DeleteHotelAsync(int id);
}