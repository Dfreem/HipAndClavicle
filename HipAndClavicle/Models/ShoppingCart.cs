
namespace HipAndClavicle.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string? CartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
        public AppUser Owner { get; set; } = default!;
    }
}
