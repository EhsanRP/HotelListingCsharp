using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Application.DTOs.Country;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Extensions;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;
using HotelListing.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Application.Services;

public class CountriesService(HotelListingDbContext context, IMapper mapper) : ICountriesService
{
    public async Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(
        CountryFilterParameters filters,
        PaginationParameters paginationParameters)
    {
        var query = context.Countries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.SearchString))
        {
            var term = filters.SearchString.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%" )|| EF.Functions.Like(c.ShortName, $"%{term}%"));
        }
        
        var countries = await query
            .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await CountryExistsAsync(id);
        return country;
    }

    public async Task<Result<GetCountryDto>> UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        try
        {
            //Checking if paylaod ID Matches DTO ID
            if (id != countryDto.Id)
            {
                return Result<GetCountryDto>.BadRequest(new Error(ErrorCodes.Validation, ErrorDescriptions.IdRouteValueMismatch()));
            }

            //Checking if the Corresponding Country Exists in Database
            var countryExistCheckResult = await CountryExistsAsync(id);
            if (!countryExistCheckResult.IsSuccess)
            {
                return countryExistCheckResult;
            }

            //Checking if New Country Name Exists in Database
            var targetNameExistCheck = await CountryExistsAsync(countryDto.Name);
            if (!targetNameExistCheck.IsSuccess)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, ErrorDescriptions.CountryAlreadyExists(countryDto.Name)));
            }

            var country = mapper.Map<Country>(countryDto);

            context.Countries.Update(country);
            await context.SaveChangesAsync();

            var updatedCountryDto = mapper.Map<GetCountryDto>(country);
            return Result<GetCountryDto>.Success(updatedCountryDto);
        }
        catch (Exception e)
        {
            return Result<GetCountryDto>.Failure();
        }
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
    {
        try
        {
            var exists = await CountryExistsAsync(countryDto.Name);
            if (!exists.IsSuccess)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, ErrorDescriptions.CountryAlreadyExists(countryDto.Name)));
            }

            var country = mapper.Map<Country>(countryDto);

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var resultDto = mapper.Map<GetCountryDto>(country);

            return Result<GetCountryDto>.Success(resultDto);
        }
        catch (Exception)
        {
            return Result<GetCountryDto>.Failure();
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        var countryExists = await CountryExistsAsync(id);
        if (!countryExists.IsSuccess)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.CountryNotFound(id)));
        }
    
        var country = mapper.Map<Country>(countryExists.Value!);
        
        context.Remove(country);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<GetCountryDto>> PatchCountryAsync(int id, JsonPatchDocument<UpdateCountryDto> patchDocument)
    {
        if (patchDocument == null)
        {
            return Result<GetCountryDto>.BadRequest(new Error(ErrorCodes.BadRequest,
                ErrorDescriptions.PatchDocumentMissing()));
        }
        
        var country = await context.Countries.FindAsync(id);
        if (country is null)
        {
            return Result<GetCountryDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.CountryNotFound(id)));
        }

        var countryDto = mapper.Map<UpdateCountryDto>(country);
        patchDocument.ApplyTo(countryDto);

        if (id != countryDto.Id)
        {
            return Result<GetCountryDto>.BadRequest(new Error(ErrorCodes.Validation, ErrorDescriptions.IdRouteValueMismatch()));
        }
        
        var duplicateExists = await CountryExistsAsync(countryDto.Name);
        if (duplicateExists.IsSuccess)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, ErrorDescriptions.CountryAlreadyExists(countryDto.Name)));
        }

        mapper.Map(countryDto, country);
        await context.SaveChangesAsync();
        
        var result = mapper.Map<GetCountryDto>(country);

        return Result<GetCountryDto>.Success(result);

    }

    private async Task<Result<GetCountryDto>> CountryExistsAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, ErrorDescriptions.CountryNotFound(id)))
            : Result<GetCountryDto>.Success(country);
    }

    private async Task<Result> CountryExistsAsync(string name)
    {
        return await context.Countries.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim())
            ? Result.Failure(new Error(ErrorCodes.NotFound, ErrorDescriptions.CountryNotFound(name)))
            : Result.Success();
    }
    
}