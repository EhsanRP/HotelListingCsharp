using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOs.Country;

namespace HotelListing.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
        CreateMap<UpdateCountryDto, Country>();
    }
}