namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public string CartId { get; set; }
        public List<ShoppingCartItemViewModel> ShoppingCartItems { get; set; }
    }

    public class ShoppingCartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Qty { get; set; }
        public double ItemPrice { get; set; }
        public string Img { get; set; }

        public ShoppingCartItemViewModel(ShoppingCartItem shoppingCartItem)
        {
            Id = shoppingCartItem.ShoppingCartItemId;
            Name = shoppingCartItem.ListingItem.ListingTitle;
            Desc = shoppingCartItem.ListingItem.ListingDescription;
            Qty = shoppingCartItem.Quantity;
            ItemPrice = shoppingCartItem.ListingItem.Price;
            // TODO: Fix displaying image for cart
            Img = "~/images/butterflys.png";
        }
    }
}
