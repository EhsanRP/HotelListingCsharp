using AutoMapper;
using HotelListing.Application.DTOs.Country;
using HotelListing.Domain;

namespace HotelListing.Application.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
        CreateMap<UpdateCountryDto, Country>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}