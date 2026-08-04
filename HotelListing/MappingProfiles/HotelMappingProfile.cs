using AutoMapper;
using HotelListing.Domain;
using HotelListing.DTOs.Hotel;

namespace HotelListing.MappingProfiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelDto>()
            .ForMember(dest => dest.CountryName, config => config.MapFrom<CountryNameResolver>());

        CreateMap<Hotel, GetHotelsDto>()
            .ForMember(dest => dest.CountryName, config => config.MapFrom(src => src.Country!.Name));
        
        CreateMap<Hotel, GetHotelSlimDto>();

        CreateMap<UpdateHotelDto, Hotel>();

        CreateMap<CreateHotelDto, Hotel>();

    }
}

public class CountryNameResolver : IValueResolver<Hotel, GetHotelDto, string>
{
    public string Resolve(Hotel source, GetHotelDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ?? string.Empty;
    }
}