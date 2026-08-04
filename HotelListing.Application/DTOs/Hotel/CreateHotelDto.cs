using System.ComponentModel.DataAnnotations;

namespace HotelListing.Application.DTOs.Hotel;

public class CreateHotelDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Address { get; set; }
    [Required] [Range(0, 5)] public double Rating { get; set; }
    [Required] public required int CountryId { get; set; }
    [Required] public decimal PerNightRate { get; set; }
}