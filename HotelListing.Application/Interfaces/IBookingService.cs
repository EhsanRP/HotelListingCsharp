using HotelListing.Application.DTOs.Booking;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface IBookingService
{
    Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId,PaginationParameters paginationParameters);
    Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto);
    Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    Task<Result<GetBookingDto>> GetBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> CancelBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> AdminCancelBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> AdminConfirmBookingAsync(int hotelId, int bookingId);
    Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAdminAsync(int hotelId,PaginationParameters paginationParameters);
}