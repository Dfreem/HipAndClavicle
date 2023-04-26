namespace HipAndClavicle.ViewModels
{
    public class AddListingVM
    {
        public List<Product> Products { get; set; } = new ();
        public List<Color> AvailableColors { get; set; } = new List<Color> ();
        public List<Listing> Listings { get; set; } = new();

        public Listing NewListing { get; set; } = new();

        public IFormFile ListingImageFile { get; set; } = default!;
        //public List<Image> ListingImages { get; set; } = default!;
        //public double ListingPrice { get; set; } = default!;
        //public List<Color> ListingColors { get; set; } = new List<Color>();
        //public Product ListingProduct { get; set; } = default!;
        //public string ListingTitle { get; set; } = default!;
        //public string ListingDescription { get; set; } = default!;
        //public int OnHand { get; set; } = default;

        public static explicit operator Listing(AddListingVM vm)
        {
            return new Listing()
            {
                Images = vm.NewListing.Images,
                Price = vm.NewListing.Price,
                Colors = vm.AvailableColors,
                ListingProduct = vm.NewListing.ListingProduct,
                ListingTitle = vm.NewListing.ListingTitle,
                ListingDescription = vm.NewListing.ListingDescription,
                OnHand = vm.NewListing.OnHand
            };
        }
    }
}
