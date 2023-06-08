namespace HipAndClavicle.ViewModels
{
    public class CheckoutVM
    {
        public Order Order { get; set; } = new();
        public ShoppingCart Cart { get; set; } = new();
        public AppUser CurrentUser { get; set; } = default!;
    }
}
