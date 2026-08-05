using HotelListing.Application.DTOs.Hotel;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Paging;
using HotelListing.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelsService hotelsService) : BaseApiController
{
    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetHotelsDto>>> GetHotels(
        [FromQuery]HotelFilterParameters filters,
        [FromQuery] PaginationParameters paginationParameters)
    {
        var hotels = await hotelsService.GetHotelsAsync(filters, paginationParameters);

        return ToActionResult(hotels);
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        var hotel = await hotelsService.GetHotelAsync(id);
        return ToActionResult(hotel);
    }

    // PUT: api/Hotels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = $"{UserRoleNames.Administrator}")]
    public async Task<ActionResult<GetHotelDto>> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        var updatedHotelDto = await hotelsService.UpdateHotelAsync(id, hotelDto);

        return ToActionResult(updatedHotelDto);
    }

    // POST: api/Hotels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = $"{UserRoleNames.Administrator}")]
    public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
    {
        var result = await hotelsService.CreateHotelAsync(hotelDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction("GetHotel", new { id = result.Value!.Id }, result);
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoleNames.Administrator}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var result = await hotelsService.DeleteHotelAsync(id);
        return ToActionResult(result);
    }

    
}