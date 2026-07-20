namespace HotelListing.DTOs.Booking;

public record UpdateBookingDto(
    int Id,
    int HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests);