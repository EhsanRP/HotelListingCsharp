using HotelListing.DTOs.Booking;
using HotelListing.Results;

namespace HotelListing.Interfaces;

public interface IBookingService
{
    Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId);
    Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto);
    Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    Task<Result<GetBookingDto>> GetBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> CancelBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> AdminCancelBookingAsync(int hotelId, int bookingId);
    Task<Result<GetBookingDto>> AdminConfirmBookingAsync(int hotelId, int bookingId);
    Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAdminAsync(int hotelId);
}