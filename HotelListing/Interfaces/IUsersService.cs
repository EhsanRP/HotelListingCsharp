using HotelListing.Common.Results;
using HotelListing.DTOs.Auth;

namespace HotelListing.Interfaces;

public interface IUsersService
{
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
    string GetUserId { get; }
}