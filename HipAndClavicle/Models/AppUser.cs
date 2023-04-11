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
    public string Address { get; set; } = default!;
    [NotMapped]
    public IList<string>? RoleNames { get; set; }
    public bool IsPersistent { get; set; } = true;
}

