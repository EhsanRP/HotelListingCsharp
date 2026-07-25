using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Constants;
using HotelListing.Data;
using HotelListing.Data.Enums;
using HotelListing.DTOs.Booking;
using HotelListing.DTOs.Hotel;
using HotelListing.Interfaces;
using HotelListing.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Services;

public class BookingService(
    HotelListingDbContext context,
    IMapper mapper,
    IUsersService usersService)
    : IBookingService
{
    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAdminAsync(int hotelId)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<IEnumerable<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var bookings = await context.Bookings
            .Where(h => h.HotelId == hotelId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetBookingDto>>.Success(bookings);    }
    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId)
    {
        var userId = usersService.GetUserId;

        var hotelExists = await HotelExists(hotelId);
        if (!hotelExists.IsSuccess)
        {
            return Result<IEnumerable<GetBookingDto>>.NotFound(new Error(ErrorCodes.NotFound,
                ErrorDescriptions.HotelNotFound(hotelId)));
        }

        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
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
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, ErrorDescriptions.BookingAlreadyCancelled()));
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
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.BookingNotFound(bookingId)));
        }
        
        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.BookingAlreadyCancelled()));
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
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.BookingNotFound(bookingId)));
        }
        
        if (booking.StatusEnum == BookingStatusEnum.Cancelled)
        {
            return Result<GetBookingDto>.NotFound(new Error(ErrorCodes.NotFound, ErrorDescriptions.BookingAlreadyCancelled()));
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

        var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;
        if (nights <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.BookingDurationInvalid()));
        }

        if (createBookingDto.Guests <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.GuestsCountInvalid(createBookingDto.Guests)));
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

        var overlaps = await context.Bookings.AnyAsync(b =>
            b.Hotel!.Id == createBookingDto.HotelId
            && b.StatusEnum != BookingStatusEnum.Cancelled
            && createBookingDto.CheckIn < b.CheckOut
            && createBookingDto.CheckOut > b.CheckOut
            && b.UserId == userId);
        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                ErrorDescriptions.OverLappingBookings()));
        }

        var totalPrice = hotel.Value!.PerNightRate * nights;
        var booking = new Booking
        {
            HotelId = createBookingDto.HotelId,
            UserId = userId,
            CheckIn = createBookingDto.CheckIn,
            CheckOut = createBookingDto.CheckOut,
            Guests = createBookingDto.Guests,
            TotalPrice = totalPrice,
            StatusEnum = BookingStatusEnum.Pending
        };

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

        var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;
        if (nights <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.BookingDurationInvalid()));
        }

        if (updateBookingDto.Guests <= 0)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation,
                ErrorDescriptions.GuestsCountInvalid(updateBookingDto.Guests)));
        }


        var overlaps = await context.Bookings.AnyAsync(b =>
            b.Hotel!.Id == updateBookingDto.HotelId
            && b.StatusEnum != BookingStatusEnum.Cancelled
            && updateBookingDto.CheckIn < b.CheckOut
            && updateBookingDto.CheckOut > b.CheckOut
            && b.UserId == userId);
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
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, ErrorDescriptions.BookingAlreadyCancelled()));
        }

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
}