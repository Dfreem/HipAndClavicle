
namespace HipAndClavicle.Models;

public class ShoppingCart
{
    public string ShoppingCartId { get; set; }
    public List<ShoppingCartItem> ShoppingCartItems { get; set; }

    //old code for model
/*    public int ShoppingCartId { get; set; }
    public List<Product> Products { get; set; } = new();
    public string OwnerId { get; set; } = default!;
    public AppUser Owner { get; set; } = default!;*/
}

