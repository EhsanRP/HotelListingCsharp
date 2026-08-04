using HotelListing.Application.DTOs.Auth;
using HotelListing.Common.Results;

namespace HotelListing.Application.Interfaces;

public interface IUsersService
{
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
    string GetUserId { get; }
}