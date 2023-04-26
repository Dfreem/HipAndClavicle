
namespace HipAndClavicle.Models;

public class ShoppingCart
{
    public ShoppingCart() 
    {
        ShoppingCartItems = new List<ShoppingCartItem>();
    }

    public string ShoppingCartId { get; set; }
    public List<ShoppingCartItem>? ShoppingCartItems { get; set; }

}

