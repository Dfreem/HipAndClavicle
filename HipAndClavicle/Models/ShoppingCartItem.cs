namespace HipAndClavicle.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int ShoppingCartId { get; set; }
        public Listing ListingItem { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
