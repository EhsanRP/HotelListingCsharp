using HotelListing.Application.DTOs.Auth;
using HotelListing.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUsersService usersService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await usersService.RegisterUserAsync(registerUserDto);

            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto)
        {
            var result = await usersService.LoginAsync(loginUserDto);
            return ToActionResult(result);
        }
    }
}