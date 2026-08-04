using System.ComponentModel.DataAnnotations;

namespace HotelListing.Application.DTOs.Country;

public class CreateCountryDto
{
    [Required] public string Name { get; set; }
    [Required] [MaxLength(5)] public string ShortName { get; set; }
}