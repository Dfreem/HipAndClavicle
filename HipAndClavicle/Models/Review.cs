namespace HipAndClavicle.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string? ReviewerId { get; set; }
        public AppUser Reviewer { get; set; } = default!;
        public string Message { get; set; } = default!;
        public int VerifiedOrderId { get; set; } = default!;
        public Order VerfiedOrder { get; set; } = default!;
        public int ReviewedProductId { get; set; } = default!;
        public Product ReviewedProduct { get; set; } = default!;
        [Range(1, 5)]
        public int StarRating { get; set; }
    }
}
