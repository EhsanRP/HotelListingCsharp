using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Constants;
using HotelListing.Data;
using HotelListing.DTOs.Country;
using HotelListing.DTOs.Hotel;
using HotelListing.Interfaces;
using HotelListing.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Services;

public class HotelsService(HotelListingDbContext context, IMapper mapper) : IHotelsService
{
    public async Task<Result<IEnumerable<GetHotelsDto>>> GetHotelsAsync()
    {
        var hotels = await context.Hotels
            .Include(h => h.Country)
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetHotelsDto>>.Success(hotels);
    }

    public async Task<Result<GetHotelDto>> GetHotelAsync(int id)
    {
        var hotelExists = await HotelExistsAsync(id);
        return hotelExists;
    }

    public async Task<Result<GetHotelDto>> UpdateHotelAsync(int id, UpdateHotelDto updateHotelDto)
    {
        try
        {
            if (id != updateHotelDto.Id)
            {
                return Result<GetHotelDto>.BadRequest(new Error(ErrorCodes.Validation,
                    "Id route value does not match payload Id"));
            }

            var isHotelExist = await HotelExistsAsync(updateHotelDto.Id);

            if (!isHotelExist.IsSuccess)
            {
                return isHotelExist;
            }

            if (updateHotelDto.CountryId != isHotelExist.Value!.CountryId)
            {
                var isCountryExit = await CountryExists(updateHotelDto.CountryId);
                if (!isCountryExit.IsSuccess)
                {
                    return Result<GetHotelDto>.NotFound(new Error(ErrorCodes.NotFound, $"Country with Id= {id} not found"));
                }
            }

            var hotel = mapper.Map<Hotel>(updateHotelDto);
            context.Hotels.Update(hotel);
            await context.SaveChangesAsync();

            var updatedHotel = mapper.Map<GetHotelDto>(hotel);
            return Result<GetHotelDto>.Success(updatedHotel);
        }
        catch (Exception e)
        {
            return Result<GetHotelDto>.Failure();
        }
    }

    public async Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto createHotelDto)
    {
        try
        {
            var isCountryExit = await CountryExists(createHotelDto.CountryId);
            if (!isCountryExit.IsSuccess)
            {
                return Result<GetHotelDto>.NotFound(new Error(ErrorCodes.NotFound,
                    $"Country with Id= {createHotelDto.CountryId} not found"));
            }

            var hotel = mapper.Map<Hotel>(createHotelDto);
            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            var resultDto = mapper.Map<GetHotelDto>(hotel);

            return Result<GetHotelDto>.Success(resultDto);
        }
        catch (Exception e)
        {
            return Result<GetHotelDto>.Failure();
        }
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        var hotelExists = await HotelExistsAsync(id);
        if (!hotelExists.IsSuccess)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound,
                $"Hotel with Id= {id} Not Found"));
        }
        
        var hotel = mapper.Map<Hotel>(hotelExists);
        
        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<Result<GetHotelDto>> HotelExistsAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .Include(h => h.Country)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return hotel is null
            ? Result<GetHotelDto>.NotFound()
            : Result<GetHotelDto>.Success(hotel);
    }

    private async Task<Result<GetCountryDto>> CountryExists(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country with id {id} not found"))
            : Result<GetCountryDto>.Success(country);
    }
}