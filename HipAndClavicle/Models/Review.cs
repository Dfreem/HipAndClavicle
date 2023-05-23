namespace HipAndClavicle.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public AppUser Reviewer { get; set; } = default!;
        public string Message { get; set; } = default!;
        public Order VerifiedOrder { get; set; } = default!;
        public Product ReviewedProduct { get; set; } = default!;
        [Range(1, 5)]
        public int StarRating { get; set; }
    }
}
