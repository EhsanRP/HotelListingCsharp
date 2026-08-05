using HotelListing.Common.Models.Enums;
using HotelListing.Common.Models.Filtering.SortingEnums;

namespace HotelListing.Common.Models.Filtering;

public class BookingFilterParameters : BaseFilterParameters
{
    public BookingStatusEnum? BookingStatus { get; set; }
    public DateOnly? CheckInAfter { get; set; }
    public DateOnly? CheckInBefore { get; set; }
    public DateOnly? CheckOutAfter { get; set; }
    public DateOnly? CheckOutBefore { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinGuests { get; set; }
    public int? MaxGuests { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }

    public BookingSortingEnum? SortBy { get; set; }
}