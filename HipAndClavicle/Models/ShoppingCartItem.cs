using HipAndClavicle.Models.JunctionTables;

namespace HipAndClavicle.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public int ShoppingCartId { get; set; }
        public Listing ListingItem { get; set; } = default!;
        public int Quantity { get; set; }
        public SetSize ItemSetSize { get; set; } = default!;
        public Color ItemColor { get; set; } = default!;
    }
}
