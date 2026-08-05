using HotelListing.Common.Models.Enums;

namespace HotelListing.Common.Models.Filtering;

public class BookingFilterParameters : BaseFilterParameters
{
    public BookingStatusEnum? BookingStatus { get; set; }
    public DateTime? CheckInFrom { get; set; }
    public DateTime? CheckInTo { get; set; }
    public DateTime? CheckOutFrom { get; set; }
    public DateTime? CheckOutTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int MinGuests { get; set; }
    public int MaxGuests { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}