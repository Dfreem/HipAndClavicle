
namespace HipAndClavicle.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public string? OwnerId { get; set; }
        public AppUser? Owner { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
