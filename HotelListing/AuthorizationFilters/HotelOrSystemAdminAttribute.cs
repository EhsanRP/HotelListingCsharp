using System.Security.Claims;
using HotelListing.Common.Constants;
using HotelListing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.AuthorizationFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HotelOrSystemAdminAttribute() : TypeFilterAttribute(typeof(HotelOrSystemAdminFilter));

public class HotelOrSystemAdminFilter(HotelListingDbContext dbContext) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;

        //User is not authorized. It's kind of redundant because of the original [Authorize] annotation on any controller
        if (httpUser?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        //If it's a system admin allow immediately
        if (httpUser!.IsInRole(UserRoleNames.Administrator))
        {
            return;
        }

        //User is Authorized. Is the user id present? It's Kind of redundant too
        var userId = httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        //try to get hotelId from route values
        context.RouteData.Values.TryGetValue("hotelId", out var hotelIdObj);
        int.TryParse(hotelIdObj?.ToString(), out var parsedHotelId);

        if (parsedHotelId == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        //If the user is admin for the specified hotel
        var isHotelAdmin = await dbContext.HotelAdmins
            .AnyAsync(q => q.UserId == userId && q.HotelId == parsedHotelId);
        if (!isHotelAdmin)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}