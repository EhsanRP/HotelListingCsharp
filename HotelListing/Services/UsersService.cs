using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelListing.Common.Constants;
using HotelListing.Common.Results;
using HotelListing.Data;
using HotelListing.DTOs.Auth;
using HotelListing.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListing.Services;

public class UsersService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    HotelListingDbContext context) : IUsersService
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

        await userManager.AddToRoleAsync(user, registerUserDto.Role);

        if (registerUserDto.Role == UserRoleNames.HotelAdmin)
        {
            {
                var hotelAdmin = context.HotelAdmins.Add(new HotelAdmin()
                {
                    UserId = user.Id,
                    HotelId = registerUserDto.AssociatedHotelId.GetValueOrDefault()
                });
                await context.SaveChangesAsync();
            }
        }

        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = registerUserDto.Role
        };

        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto loginUserDto)
    {
        var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "no username found"));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid password"));
        }

        //issue a token
        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    public string GetUserId => httpContextAccessor?
        .HttpContext?
        .User?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        //basic user claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName)
        };

        //set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        claims = claims.Union(roleClaims).ToList();

        //set JWT key credentials
        var jwtKey = configuration["JwtSettings:Key"];
        if (jwtKey == null)
        {
            throw new Exception("Some Programmer Fucked the AppSettings UP!");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //create encoded token
        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])
            ),
            signingCredentials: credentials
        );

        //return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}