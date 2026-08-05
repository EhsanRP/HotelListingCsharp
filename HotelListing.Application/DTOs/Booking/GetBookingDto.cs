using HotelListing.Common.Models.Enums;

namespace HotelListing.Application.DTOs.Booking;

public class GetBookingDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public BookingStatusEnum Status { get; set; } = BookingStatusEnum.Null;
}