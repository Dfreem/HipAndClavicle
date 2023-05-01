namespace HipAndClavicle.ViewModels
{
    public class CustReviewVM
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Review Review { get; set; }
        public AppUser Reviewer { get; set; }
        public int ListingId { get; set; }

    }
}
