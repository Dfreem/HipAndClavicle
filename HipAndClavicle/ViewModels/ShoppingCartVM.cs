
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HipAndClavicle.ViewModels;

public class ShoppingCartViewModel
{
    public ShoppingCart ShoppingCart { get; set; } = new();
    public List<OrderItem> ShoppingCartItems { get; set; } = new();
    //public ShoppingCartItemViewModel(SimpleCartItem simpleCartItem)
    //{
    //    Id = simpleCartItem.Id;
    //    Name = simpleCartItem.Name;
    //    Desc = simpleCartItem.Desc;
    //    Qty = simpleCartItem.Qty;
    //    ItemPrice = simpleCartItem.ItemPrice;
    //    // TODO: Fix displaying image for cart
    //    Img = "~/images/hp-logo.png";
    //}
    // This is a simple version of the ShoppingCartItemViewModel that is used for saving a cart to a cookie
    public class SimpleCartItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Desc { get; set; } = default!;
        public double ItemPrice { get; set; }
        public int ListingId { get; set; }
        public int Qty { get; set; }
    }

    public class SimpleShoppingCart
    {
        public List<SimpleCartItem> Items { get; set; } = new();
    }
}

