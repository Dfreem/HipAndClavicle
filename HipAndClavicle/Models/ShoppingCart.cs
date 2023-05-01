
namespace HipAndClavicle.Models;

public class ShoppingCart
{
    public int ShoppingCartId { get; set; }
    public List<ShoppingCartItem> ShoppingCartItems { get; set; }
    public AppUser Owner { get; set; } = default!;
    public double CartTotal { get; set; }

}

