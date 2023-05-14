namespace HipAndClavicle.Models;

public class ShippingAddress
{
    public int ShippingAddressId { get; set; }
    [Display(Name = "Line 1")]
    public string AddressLine1 { get; set; } = default!;
    [Display(Name = "Line 2")]
    public string? AddressLine2 { get; set; } = default!;
    public string Country { get; set; } = "us";
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; } = default!;
    [Display(Name = "City")]
    public string CityTown { get; set; } = default!;
    [Display(Name = "State")]
    public State StateAbr { get; set; } = default!;
    [DataType(DataType.PostalCode)]
    [Display(Name = "Zip-code")]
    public string PostalCode { get; set; } = default!;
    public bool Residential { get; set; } = true;
    public string Name { get; set; } = "Not Set";
    
    public string? AppUserId { get; set; }
    public List<AppUser> Residents { get; set; } = new()!;

}
