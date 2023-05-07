namespace HipAndClavicle.Models;

public class AppUser : IdentityUser
{
    [Required]
    [MinLength(1, ErrorMessage = "first name must atleast have one character")]
    [MaxLength(20, ErrorMessage = "limit first name to 20 characters")]
    public string FName { get; set; } = default!;
    [Required]
    [MinLength(1, ErrorMessage = "first name must atleast have one character")]
    [MaxLength(20, ErrorMessage = "limit last name to 20 characters")]
    public string LName { get; set; } = default!;
    public bool IsPersistent { get; set; } = true;
    public int? ShippingAddressId { get; set; }
    public ShippingAddress? Address { get; set; }
    public bool IsActive { get; set; } = true;
}

