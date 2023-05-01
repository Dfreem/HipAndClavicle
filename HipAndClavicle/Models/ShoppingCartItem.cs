namespace HipAndClavicle.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int ShoppingCartId { get; set; }
        public Product Product { get; set; } = default!;
        public ShoppingCart ShoppingCart { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
