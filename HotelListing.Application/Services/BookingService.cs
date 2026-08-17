using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Application.DTOs.Booking;
using HotelListing.Application.DTOs.Hotel;
using HotelListing.Application.Interfaces;
using HotelListing.Common.Constants;
using HotelListing.Common.Models.Enums;
using HotelListing.Common.Models.Extensions;
using HotelListing.Common.Models.Filtering;
using HotelListing.Common.Models.Filtering.SortingEnums;
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
    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAdminAsync(
        int hotelId,
        BookingFilterParameters filters,
        PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<PagedResult<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var query = AppendFilters(hotelId, filters);
        var bookings = await query
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<PagedResult<GetBookingDto>>> GetBookingsForHotelAsync(
        int hotelId,
        BookingFilterParameters filters,
        PaginationParameters paginationParameters)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<PagedResult<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var query = AppendFilters(hotelId, filters);
        var bookings = await query
            .Where(b => b.UserId == userId)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> GetBookingAsync(int hotelId, int bookingId)
    {
        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .AsNoTracking()
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
            .AsNoTracking()
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

    private IQueryable<Booking> AppendFilters(int hotelId, BookingFilterParameters filters)
    {
        var query = context.Bookings.Where(b => b.Id == hotelId);

        if (filters.BookingStatus.HasValue)
            query = query.Where(b => b.StatusEnum == filters.BookingStatus.Value);

        if (filters.CheckInAfter.HasValue)
            query = query.Where(b => b.CheckIn >= filters.CheckInAfter);

        if (filters.CheckInBefore.HasValue)
            query = query.Where(b => b.CheckIn <= filters.CheckInBefore);

        if (filters.CheckOutAfter.HasValue)
            query = query.Where(b => b.CheckOut >= filters.CheckOutAfter);

        if (filters.CheckOutBefore.HasValue)
            query = query.Where(b => b.CheckOut <= filters.CheckOutBefore);

        if (filters.MinGuests.HasValue)
            query = query.Where(b => b.Guests >= filters.MinGuests);

        if (filters.MaxGuests.HasValue)
            query = query.Where(b => b.Guests <= filters.MaxGuests);

        if (filters.MinPrice.HasValue)
            query = query.Where(b => b.TotalPrice >= filters.MinPrice);

        if (filters.MaxPrice.HasValue)
            query = query.Where(b => b.TotalPrice <= filters.MaxPrice);


        query = filters.SortBy switch
        {
            BookingSortingEnum.Checkin => filters.SortDescending
                ? query.OrderByDescending(b => b.CheckIn)
                : query.OrderBy(b => b.CheckIn),
            BookingSortingEnum.Checkout => filters.SortDescending
                ? query.OrderByDescending(b => b.CheckOut)
                : query.OrderBy(b => b.CheckOut),
            BookingSortingEnum.Price => filters.SortDescending
                ? query.OrderByDescending(b => b.TotalPrice)
                : query.OrderBy(b => b.TotalPrice),
            BookingSortingEnum.Created => filters.SortDescending
                ? query.OrderByDescending(b => b.CreatedAt)
                : query.OrderBy(b => b.CreatedAt),
            _ => query.OrderBy(b => b.CheckIn)
        };

        return query;
    }
}