namespace HipAndClavicle.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItemViewModel> ShoppingCartItems { get; set; }
        public double CartTotal { get; set; }
        //public bool IsUserLoggedIn { get; set; }
    }

    public class ShoppingCartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public Image Img { get; set; }
        public double ItemPrice { get; set; }
        public int Qty { get; set; }
    }

    /*public class ListingViewModel
    {
        public int ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string ListingDescription { get; set; }
        public string ListingImage { get; set; }
        public double ListingPrice { get; set; }
    }*/
}
