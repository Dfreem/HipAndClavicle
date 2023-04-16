namespace HipAndClavicle.Data
{
    public interface ICustRepo
    {
        //Get all 
        public Task<List<Listing>> GetAllListingsAsync();

        //Get specific
        public Task<Listing> GetListingByIdAsync(int listingId);
        public Task<List<Color>> GetColorsByColorFamilyNameAsync(string colorFamilyName);
        public Task<List<Listing>> GetListingsByColorFamilyAsync(string colorFamilyName);

        //Make Updates
        public Task AddColorFamilyAsync(ColorFamily colorFamily);



    }
}
