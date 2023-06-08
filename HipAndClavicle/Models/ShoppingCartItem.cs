namespace HipAndClavicle.Models
{
    public class ShoppingCartItem : OrderItem
    {
        public int ShoppingCartItemId { get; set; }
        public OrderItem ListingItem { get; set; } = default!;
    }
}
