using HotelListing.Application.DTOs.Country;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountriesController(ICountriesService countriesService) : BaseApiController
{
    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCountriesDto>>> GetCountries(
        [FromQuery] CountryFilterParameters filters,
        [FromQuery] PaginationParameters paginationParameters)
    {
        var countries = await countriesService.GetCountriesAsync(filters,paginationParameters);
        return ToActionResult(countries);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var country = await countriesService.GetCountryAsync(id);

        return ToActionResult(country);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = UserRoleNames.Administrator)]
    public async Task<ActionResult<GetCountryDto>> PutCountry(int id, UpdateCountryDto countryDto)
    {
        var updatedCountry = await countriesService.UpdateCountryAsync(id, countryDto);

        return ToActionResult(updatedCountry);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = UserRoleNames.Administrator)]
    public async Task<ActionResult<GetCountryDto>> PatchCountry(
        int id,
        [FromBody] JsonPatchDocument<UpdateCountryDto> patchDocument)
    {
        var result = await countriesService.PatchCountryAsync(id, patchDocument);
        return ToActionResult(result);
    }
    

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = UserRoleNames.Administrator)]
    public async Task<ActionResult<CreateCountryDto>> PostCountry(CreateCountryDto countryDto)
    {
        var result = await countriesService.CreateCountryAsync(countryDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction(nameof(GetCountry), new { id = result.Value!.Id }, result);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var result = await countriesService.DeleteCountryAsync(id);
        return ToActionResult(result);
    }
}