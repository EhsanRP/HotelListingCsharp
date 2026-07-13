using HotelListing.Constants;
using HotelListing.Data;
using HotelListing.DTOs.Auth;
using HotelListing.Interfaces;
using HotelListing.Results;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Services;

public class UsersService(UserManager<ApplicationUser> userManager) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Email,
            LastName = registerUserDto.LastName,
            FirstName = registerUserDto.FirstName,
        };
        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => new Error(ErrorCodes.BadRequest, error.Description))
                .ToArray();
            return Result<RegisteredUserDto>.Failure(errors);
        }

        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto loginUserDto)
    {
        var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest,"Invalid Credentials"));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest,"Invalid Credentials"));
        }
            
        return Result<string>.Success("Login Successful");
    }
    
}