using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOs.Country;

public class UpdateCountryDto
{
    [Required] public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] [MaxLength(5)] public string ShortName { get; set; }
}