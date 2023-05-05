
namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public string CartId { get; set; }
        public List<ShoppingCartItemViewModel> ShoppingCartItems { get; set; }
        public double CartTotal => ShoppingCartItems.Sum(x => x.ItemTotal);

    }

    public class ShoppingCartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Qty { get; set; }
        public double ItemPrice { get; set; }
        public string Img { get; set; }
        public double ItemTotal => Qty * ItemPrice;

        public ShoppingCartItemViewModel(ShoppingCartItem shoppingCartItem)
        {
            Id = shoppingCartItem.ShoppingCartItemId;
            Name = shoppingCartItem.ListingItem.ListingTitle;
            Desc = shoppingCartItem.ListingItem.ListingDescription;
            Qty = shoppingCartItem.Quantity;
            ItemPrice = shoppingCartItem.ListingItem.Price;
            // TODO: Fix displaying image for cart
            Img = "~/images/hp-logo.png";
        }

        public ShoppingCartItemViewModel(SimpleCartItem simpleCartItem) 
        {
            Id = simpleCartItem.Id;
            Name = simpleCartItem.Name;
            Desc = simpleCartItem.Desc;
            Qty = simpleCartItem.Qty;
            ItemPrice = simpleCartItem.ItemPrice;
            // TODO: Fix displaying image for cart
            Img = "~/images/hp-logo.png";
        }

    }

    // This is a simple version of the ShoppingCartItemViewModel that is used for saving a cart to a cookie
    public class SimpleCartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public double ItemPrice { get; set; }
        public int ListingId { get; set; }
        public int Qty { get; set; }
    }

    public class SimpleShoppingCart
    {
        public List<SimpleCartItem> Items { get; set; }
    }
}
