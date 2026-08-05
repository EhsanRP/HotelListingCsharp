using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Application.DTOs.Booking;
using HotelListing.Application.DTOs.Hotel;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Enums;
using HotelListing.Common.Models.Extensions;
using HotelListing.Common.Models.Paging;
using HotelListing.Common.Results;
using HotelListing.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Application.Services;

public class BookingService(
    HotelListingDbContext context,
    IMapper mapper,
    IUsersService usersService)
    : IBookingService
{
    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAdminAsync(int hotelId,PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<PagedResult<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var bookings = await context.Bookings
            .Where(h => h.HotelId == hotelId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId,PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<PagedResult<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> GetBookingAsync(int hotelId, int bookingId)
    {
        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

        if (booking is null)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingNotFound(bookingId)));
        }

        return Result<GetBookingDto>.Success(booking);
    }

    public async Task<Result<GetBookingDto>> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.GetUserId;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, ErrorDescriptions.LoginRequired()));
        }

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);

        if (booking is null)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingNotFound(bookingId)));
        }

        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                ErrorDescriptions.BookingAlreadyCancelled()));
        }

        booking.StatusEnum = BookingStatusEnum.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(result);
    }

    public async Task<Result<GetBookingDto>> AdminCancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.GetUserId;

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);
        if (booking is null)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingNotFound(bookingId)));
        }

        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingAlreadyCancelled()));
        }

        booking.StatusEnum = BookingStatusEnum.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(result);
    }

    public async Task<Result<GetBookingDto>> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.GetUserId;

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);
        if (booking is null)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingNotFound(bookingId)));
        }

        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingAlreadyCancelled()));
        }

        booking.StatusEnum = BookingStatusEnum.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(result);
    }

    public async Task<Result<GetBookingDto>> CreateBookingAsync(int hotelId, CreateBookingDto createBookingDto)
    {
        var userId = usersService.GetUserId;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, ErrorDescriptions.LoginRequired()));
        }

        if (hotelId != createBookingDto.HotelId)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.IdRouteValueMismatch()));
        }

        var hotel = await HotelExists(hotelId);
        if (!hotel.IsSuccess)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var overlaps = await IsOverLap(hotelId, userId, createBookingDto.CheckIn, createBookingDto.CheckOut);
        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                ErrorDescriptions.OverLappingBookings()));
        }

        var totalPrice = hotel.Value!.PerNightRate *
                         (createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber);
        var booking = mapper.Map<Booking>(createBookingDto);
        booking.UserId = userId;
        booking.TotalPrice = totalPrice;

        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(result);
    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId,
        UpdateBookingDto updateBookingDto)
    {
        var userId = usersService.GetUserId;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, ErrorDescriptions.LoginRequired()));
        }

        if (bookingId != updateBookingDto.Id)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.IdRouteValueMismatch()));
        }

        if (hotelId != updateBookingDto.HotelId)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.IdRouteValueMismatch()));
        }

        var overlaps = await IsOverLap(hotelId, userId, updateBookingDto.CheckIn, updateBookingDto.CheckOut);
        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                ErrorDescriptions.OverLappingBookings()));
        }

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);

        if (booking is null)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.BookingNotFound(bookingId)));
        }

        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                ErrorDescriptions.BookingAlreadyCancelled()));
        }

        var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;
        var perNightRate = booking.Hotel!.PerNightRate;
        booking = mapper.Map(updateBookingDto, booking);
        booking.TotalPrice = perNightRate * nights;
        await context.SaveChangesAsync();

        var result = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(result);
    }

    private async Task<Result<GetHotelDto>> HotelExists(int hotelId)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == hotelId)
            .Include(h => h.Country)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return hotel is null
            ? Result<GetHotelDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.HotelNotFound(hotelId)))
            : Result<GetHotelDto>.Success(hotel);
    }

    private async Task<bool> IsOverLap(int hotelId, string userId, DateOnly checkin, DateOnly checkout,
        int? bookingId = null)
    {
        var query = context.Bookings
            .Where(b =>
                b.Hotel!.Id == hotelId
                && b.StatusEnum != BookingStatusEnum.Cancelled
                && checkin < b.CheckOut
                && checkout > b.CheckOut
                && b.UserId == userId)
            .AsQueryable();

        if (bookingId.HasValue)
        {
            query = query.Where(q => q.Id != bookingId.Value);
        }

        return await query.AnyAsync();
    }
}