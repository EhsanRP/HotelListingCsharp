namespace HotelListing.Application.DTOs.Hotel;

public record GetHotelSlimDto(
    int Id,
    string Name,
    string Address,
    double Rating
);