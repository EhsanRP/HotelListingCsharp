using AutoMapper;
using HotelListing.Application.DTOs.Booking;
using HotelListing.Domain;

namespace HotelListing.Application.MappingProfiles;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetBookingDto>()
            .ForMember(dest => dest.HotelName, config => config.MapFrom(src => src.Hotel!.Name));
            //.ForMember(dest => dest.Status, config => config.MapFrom(src => src.StatusEnum.ToString()));
        
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.Id, config => config.Ignore())
            .ForMember(dest => dest.UserId, config => config.Ignore())
            .ForMember(dest => dest.TotalPrice, config => config.Ignore())
            .ForMember(dest => dest.StatusEnum, config => config.Ignore())
            .ForMember(dest => dest.CreatedAt, config => config.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, config => config.Ignore())
            .ForMember(dest => dest.Hotel, config => config.Ignore());
        
        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(dest => dest.Id, config => config.Ignore())
            .ForMember(dest => dest.UserId, config => config.Ignore())
            .ForMember(dest => dest.TotalPrice, config => config.Ignore())
            .ForMember(dest => dest.StatusEnum, config => config.Ignore())
            .ForMember(dest => dest.CreatedAt, config => config.Ignore())
            .ForMember(dest => dest.UpdatedAt, config => config.MapFrom(_=>DateTime.UtcNow))
            .ForMember(dest => dest.Hotel, config => config.Ignore());
    }
}