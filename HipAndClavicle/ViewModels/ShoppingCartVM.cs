namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> Items { get; set; }
        public double TotalPrice => Items.Sum(x => x.TotalPrice);
    }

    public class ShoppingCartItemViewModel
    {
        public int ShoppingCartItemId { get; set; }
        public int ListingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public List<Image> Images { get; set; }
        public double TotalPrice => Quantity * Price;
    }
}
