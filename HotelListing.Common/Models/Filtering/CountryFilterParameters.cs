namespace HotelListing.Common.Models.Filtering;

public class CountryFilterParameters : BaseFilterParameters
{
    public string? CountryName { get; set; }
    public bool? HasHotelsOnly { get; set; }
}