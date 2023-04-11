namespace HipAndClavicle.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int? ReviewerId { get; set; } = default!;
        public string Message { get; set; } = default!;
        public int VerifiedOrderId { get; set; } = default!;
        public int? ReviewedProductId { get; set; } = default!;
    }
}
