namespace HipAndClavicle.ViewModels
{
    public class ProductListingVM
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Listing> Listings { get; set; } = new List<Listing>();
    }
}
