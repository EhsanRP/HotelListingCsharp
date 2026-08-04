using HotelListing.Domain.Enums;

namespace HotelListing.Domain;

public class Booking
{
    public int Id { get; set; }
    
    public required int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public BookingStatusEnum StatusEnum { get; set; } = BookingStatusEnum.Pending;
}