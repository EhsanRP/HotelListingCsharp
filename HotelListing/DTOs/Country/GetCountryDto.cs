using System.ComponentModel.DataAnnotations;
using HotelListing.DTOs.Hotel;

namespace HotelListing.DTOs.Country;

public record GetCountryDto(
    int Id,
    string Name,
    string ShortName,
    IList<GetHotelSlimDto> Hotels
);