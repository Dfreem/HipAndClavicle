namespace HipAndClavicle.ViewModels
{
    public class CustListingVM
    {
        public AppUser User { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public Listing Listing { get; set; } = default!;
        public bool ProductIsPurchaced { get; set; } = default!;
        public ShoppingCart ShoppingCart { get; set; } = default!;
    }
}
