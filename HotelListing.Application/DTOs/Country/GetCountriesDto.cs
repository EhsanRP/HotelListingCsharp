namespace HotelListing.Application.DTOs.Country;

public record GetCountriesDto(
    int Id,
    string Name,
    string ShortName
);