namespace HipAndClavicle.ViewModels
{
    public class AddListingVM
    {
        public List<Product> Products { get; set; } = new ();
        public List<Color> AvailableColors { get; set; }
        public Listing Listing { get; set; } = new();
    }
}
