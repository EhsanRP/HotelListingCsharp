using HotelListing.Application.DTOs.Hotel;

namespace HotelListing.Application.DTOs.Country;

public record GetCountryDto(
    int Id,
    string Name,
    string ShortName,
    IList<GetHotelSlimDto> Hotels
);