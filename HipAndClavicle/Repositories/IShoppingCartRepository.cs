using System.Threading.Tasks;
using HipAndClavicle.Models;

namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string shoppingCartId);
        Task<ShoppingCartItem> GetCartItemAsync(int cartItemId);
        Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task DeleteItemAsync(int itemId);
    }
}
