namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartVM
    {
        public int ShoppingCartId { get; set; }
        public List<Product> Products { get; set; }
        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }
        public double TotalPrice { get; set; }
    }
}
