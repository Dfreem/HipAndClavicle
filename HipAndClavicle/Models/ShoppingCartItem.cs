namespace HipAndClavicle.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public string ShoppingCartId { get; set; }
        public int ListingId { get; set; }
        public Listing Listing { get; set; }
        public int Quantity { get; set; }
    }
}
