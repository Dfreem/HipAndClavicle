namespace HipAndClavicle.ViewModels
{
    public class AddListingVM
    {
        public List<Product> Products { get; set; } = new ();
        public List<Color> AvailableColors { get; set; } = new List<Color> ();


        public IFormFile ListingImageFile { get; set; } = default!;
        public List<Image> ListingImages { get; set; } = default!;
        public double ListingPrice { get; set; } = default!;
        public List<Color> ListingColors { get; set; } = new List<Color>();
        public Product ListingProduct { get; set; } = default!;
        public string ListingTitle { get; set; } = default!;
        public string ListingDescription { get; set; } = default!;
        public int OnHand { get; set; } = default;

        public static explicit operator Listing(AddListingVM vm)
        {
            return new Listing()
            {
                Images = vm.ListingImages,
                Price = vm.ListingPrice,
                Colors = vm.AvailableColors,
                ListingProduct = vm.ListingProduct,
                ListingTitle = vm.ListingTitle,
                ListingDescription = vm.ListingDescription,
                OnHand = vm.OnHand
            };
        }
    }
}
