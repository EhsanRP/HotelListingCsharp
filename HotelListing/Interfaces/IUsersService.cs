using HotelListing.DTOs.Auth;
using HotelListing.Results;

namespace HotelListing.Interfaces;

public interface IUsersService
{
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
    string GetUserId { get; }
}