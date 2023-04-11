
namespace HipAndClavicle.Models;

public class ShoppingCart
{

    public int ShoppingCartId { get; set; }
    public List<Product> Products { get; set; } = new();
    public string OwnerId { get; set; } = default!;
    public AppUser Owner { get; set; } = default!;
    public Order CurrentOrder { get; set; } = default!;

}

