namespace HipAndClavicle.Data
{
    public interface ICustRepo
    {
        //Get all 
        public Task<List<Listing>> GetAllListings();

        //Get specific
        public Task<List<Listing>> GetItemsByColorFamily(string colorFamily);

        //Update db
        public Task UpdateColorFamilyAsync(ColorFamily colorFamily);



    }
}
