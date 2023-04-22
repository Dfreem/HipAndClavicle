namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> Items { get; set; }
        public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
    }

    public class ShoppingCartItemViewModel
    {
        public int ShoppingCartItemId { get; set; }
        public int ListingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
