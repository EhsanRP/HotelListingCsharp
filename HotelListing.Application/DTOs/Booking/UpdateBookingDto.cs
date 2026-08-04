using System.ComponentModel.DataAnnotations;

namespace HotelListing.Application.DTOs.Booking;

public record UpdateBookingDto(
    [Required] int Id,
    [Required] int HotelId,
    
    [Required, Range(minimum: 1, maximum: 10)]
    int Guests,
        
    DateOnly CheckIn,
    DateOnly CheckOut
    
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut <= CheckIn)
        {
            yield return new ValidationResult("CheckOut must be after CheckIn", [nameof(CheckOut), nameof(CheckIn)]);
        }
    }
}