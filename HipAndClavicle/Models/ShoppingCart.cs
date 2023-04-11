
namespace HipAndClavicle.Models;

public class ShoppingCart
{

    public int ShoppingCartId { get; set; }
    public List<Product> Products { get; set; } = new();
    public string OwnerId { get; set; } = default!;
    public AppUser Owner { get; set; } = default!;
<<<<<<< HEAD

=======
>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
}

