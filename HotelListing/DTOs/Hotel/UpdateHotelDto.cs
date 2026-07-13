using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOs.Hotel;

public class UpdateHotelDto
{
    [Required] public int Id { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Address { get; set; }
    [Required] [Range(0, 5)] public double Rating { get; set; }
    [Required] public required int CountryId { get; set; }
}