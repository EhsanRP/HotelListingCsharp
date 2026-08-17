using Microsoft.Extensions.Caching.Memory;

namespace HotelListing.Common.Constants;

public class CacheOptions
{
    public static MemoryCacheEntryOptions CountryCacheOptions =>
        new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
}