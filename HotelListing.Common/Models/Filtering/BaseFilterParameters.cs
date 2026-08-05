namespace HotelListing.Common.Models.Filtering;

public abstract class BaseFilterParameters
{
    public string? SearchString { get; set; }
    public bool SortDescending { get; set; } = false;
}