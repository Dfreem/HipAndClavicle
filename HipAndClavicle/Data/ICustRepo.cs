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

        //Make Updates
        public Task AddColorFamilyAsync(ColorFamily colorFamily);
        public Task AddListingAsync(Listing listing);
        public Task AddListingImageAsync(Image image);
        public Task AddColorToListing(Listing listing, Color color);
    }
}
