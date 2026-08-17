using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Application.DTOs.Country;
using HotelListing.Application.DTOs.Hotel;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Extensions;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Filtering.SortingEnums;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;
using HotelListing.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Application.Services;

public class HotelsService(HotelListingDbContext context, IMapper mapper) : IHotelsService
{
    public async Task<Result<PagedResult<GetHotelsDto>>> GetHotelsAsync(
        HotelFilterParameters filters,
        PaginationParameters paginationParameters)
    {
        var query = context.Hotels.AsQueryable();
        if (filters.CountryId.HasValue) query = query.Where(h => h.CountryId == filters.CountryId);
        if (filters.MinRating.HasValue) query = query.Where(h => h.Rating >= filters.MinRating.Value);
        if (filters.MaxRating.HasValue) query = query.Where(h => h.Rating <= filters.MaxRating.Value);
        if (filters.MinPrice.HasValue) query = query.Where(h => h.PerNightRate >= filters.MinPrice.Value);
        if (filters.MaxPrice.HasValue) query = query.Where(h => h.PerNightRate <= filters.MaxPrice.Value);
        if (!string.IsNullOrWhiteSpace(filters.Location))
        {
            var location = filters.Location.Trim();
            query = query.Where(h => EF.Functions.Like(h.Address, $"%{location}%"));
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchString))
        {
            var term = filters.SearchString.Trim();
            query = query.Where(h => EF.Functions.Like(h.Name, $"%{term}%" )|| EF.Functions.Like(h.Address, $"%{term}%"));
        }

        query = filters.HotelSorting switch
        {
            HotelSortingEnum.Name => filters.SortDescending
                ? query.OrderByDescending(h => h.Name)
                : query.OrderBy(h => h.Name),
            HotelSortingEnum.Rating => filters.SortDescending
                ? query.OrderByDescending(h => h.Rating)
                : query.OrderBy(h => h.Rating),
            HotelSortingEnum.Price => filters.SortDescending
                ? query.OrderByDescending(h => h.PerNightRate)
                : query.OrderBy(h => h.PerNightRate),
            _ => query.OrderBy(h => h.Name)
        };

        var hotels = await query
            .Include(h => h.Country)
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetHotelsDto>>.Success(hotels);
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
                    ErrorDescriptions.IdRouteValueMismatch()));
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
                    return Result<GetHotelDto>.NotFound(new Error(ErrorCodes.NotFound,
                        ErrorDescriptions.CountryNotFound(updateHotelDto.CountryId)));
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
                    ErrorDescriptions.CountryNotFound(createHotelDto.CountryId)));
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
                ErrorDescriptions.HotelNotFound(id)));
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
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return hotel is null
            ? Result<GetHotelDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.HotelNotFound(id)))
            : Result<GetHotelDto>.Success(hotel);
    }

    private async Task<Result<GetCountryDto>> CountryExists(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, ErrorDescriptions.CountryNotFound(id)))
            : Result<GetCountryDto>.Success(country);
    }
}