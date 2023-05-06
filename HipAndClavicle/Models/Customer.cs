namespace HipAndClavicle.Models;

public class Customer : AppUser
{
    public List<Order> OrderHistory { get; set; } = new();
    public ShoppingCart? Cart { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Address cannot be more than 100 characters or empty", MinimumLength = 1)]
    public override string? Address { get => base.Address; set => base.Address = value; }
}