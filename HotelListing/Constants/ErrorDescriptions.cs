namespace HotelListing.Constants;

public class ErrorDescriptions
{
    
    //General
    public static string IdRouteValueMismatch() => "Id route value does not match payload Id";
    public static string LoginRequired() => "Please login to perform this action";
    public static string AccessDenied() => "You don't have access to do this action";
    
    //Hotel
    public static string HotelNotFound(int id) => $"Hotel with id={id} not found";
    
    
    //Country
    public static string CountryNotFound(int id) => $"Country with id={id} not found";
    public static string CountryNotFound(string name) => $"Country with name={name} not found";
    public static string CountryAlreadyExists(string name) => $"Country with name={name} already exists";
    
    
    //Booking
    public static string BookingDurationInvalid() => "Check-in and Check-out date are invalid";
    public static string GuestsCountInvalid(int count) => $"The guests count={count} is invalid";
    public static string OverLappingBookings() => "The selected dates overlap with an existing booking";
    public static string BookingNotFound(int id) => $"Booking with id={id} not found";
    public static string BookingAlreadyCancelled() => "Booking is Cancelled";
}