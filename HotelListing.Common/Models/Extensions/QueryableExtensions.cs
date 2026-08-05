using HotelListing.Common.Models.Paging;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Common.Models.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> source,
        PaginationParameters paginationParameters)
    {
        var skipCount = (paginationParameters.PageNumber - 1) * paginationParameters.PageSize;

        var totalCount = await source.CountAsync();
        var data = await source
            .Skip(skipCount)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        var metadata = new PaginationMetadata()
        {
            CurrentPage = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = paginationParameters.PageNumber < totalPages,
            HasPreviousPage = paginationParameters.PageNumber > 1
        };

        return new PagedResult<T>
        {
            Data = data,
            Metadata = metadata
        };
    }
}