namespace HipAndClavicle.Repositories
{
    public interface ICustRepo
    {
        //Get all 
        public Task<List<Listing>> GetAllListingsAsync();
        public Task<List<Product>> GetAllProductsAsync();
        public Task<List<Color>> GetAllColorsAsync();


        //Get specific
        public Task<Listing> GetListingByIdAsync(int listingId);
        public Task<List<Color>> GetColorsByColorFamilyNameAsync(string colorFamilyName);
        public Task<List<Listing>> GetListingsByColorFamilyAsync(string colorFamilyName);
        public Task<Color> GetColorByIdAsync(int colorId);
        public Task<Product> GetProductByIdAsync(int productId);



        //Make Updates
        public Task AddColorFamilyAsync(ColorFamily colorFamily);
        public Task AddListingAsync(Listing listing);
        public Task AddListingImageAsync(Image image);
        public Task AddColorToListing(Listing listing, Color color);
        public Task AddReviewAsync(CustReviewVM crVM);


        //Checks
        public Task<bool> UserPurchasedProduct(int productId, AppUser currentUser);
    }
}
