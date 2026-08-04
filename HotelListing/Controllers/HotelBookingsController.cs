using HotelListing.Application.DTOs.Booking;
using HotelListing.Application.Interfaces;
using HotelListing.AuthorizationFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[Route("api/hotels/{hotelId:int}/bookings")]
[ApiController]
[Authorize]
public class HotelBookingsController(IBookingService bookingService) : BaseApiController
{
    [HttpGet("admin")]
    [HotelOrSystemAdmin]
    public async Task<ActionResult<IEnumerable<GetBookingDto>>> GetBookings([FromRoute] int hotelId)
    {
        var bookings = await bookingService.GetBookingsForHotelAdminAsync(hotelId);
        return ToActionResult(bookings);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetBookingDto>>> GetBookingsForHotel(int hotelId)
    {
        var bookings = await bookingService.GetBookingsForHotelAsync(hotelId);
        return ToActionResult(bookings); 
    }

    [HttpGet("{bookingId:int}")]
    public async Task<ActionResult<GetBookingDto>> GetBooking(
        [FromRoute] int hotelId,
        [FromRoute] int bookingId)
    {
        var booking = await bookingService.GetBookingAsync(hotelId, bookingId);
        return ToActionResult(booking);
    }

    [HttpPost]
    public async Task<ActionResult<GetBookingDto>> CreateBooking(
        [FromRoute] int hotelId,
        [FromBody] CreateBookingDto createBookingDto)
    {
        foreach (var claim in HttpContext.User.Claims)
        {
            Console.WriteLine($"{claim.Type} = {claim.Value}");
        }

        var result = await bookingService.CreateBookingAsync(hotelId, createBookingDto);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}")]
    public async Task<ActionResult<GetBookingDto>> UpdateBooking(
        [FromRoute] int hotelId,
        [FromRoute] int bookingId,
        [FromBody] UpdateBookingDto updateBookingDto)
    {
        var result = await bookingService.UpdateBookingAsync(hotelId, bookingId, updateBookingDto);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}/cancel")]
    public async Task<ActionResult<GetBookingDto>> CancelBooking(
        [FromRoute] int hotelId,
        [FromRoute] int bookingId
    )
    {
        var result = await bookingService.CancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}/admin/cancel")]
    [HotelOrSystemAdmin]
    public async Task<ActionResult<GetBookingDto>> AdminCancelBooking(
        [FromRoute] int hotelId,
        [FromRoute] int bookingId
    )
    {
        var result = await bookingService.AdminCancelBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }

    [HttpPut("{bookingId:int}/admin/confirm")]
    [HotelOrSystemAdmin]
    
    public async Task<ActionResult<GetBookingDto>> AdminConfirmBooking(
        [FromRoute] int hotelId,
        [FromRoute] int bookingId
    )
    {
        var result = await bookingService.AdminConfirmBookingAsync(hotelId, bookingId);
        return ToActionResult(result);
    }
}