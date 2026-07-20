using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOs.Booking;

namespace HotelListing.MappingProfiles;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetBookingDto>()
            .ForMember(
                dest => dest.HotelName,
                config => config.MapFrom(src => src.Hotel!.Name));//.ForMember(d=> d.Status , config => config.MapFrom(src => src.StatusEnum.ToString()));
        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(
                dest => dest.UpdatedAt,
                config => config.MapFrom(_ => DateTime.UtcNow));
        CreateMap<CreateBookingDto, Booking>();
    }
}