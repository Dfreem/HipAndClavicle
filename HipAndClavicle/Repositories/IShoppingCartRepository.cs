using System.Threading.Tasks;
using HipAndClavicle.Models;

namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string shoppingCartId);
        Task<ShoppingCart> GetShoppingCartByUser(string userId);
        Task<List<ShoppingCartItem>> GetItemsAsync(string userId);
        Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item, string userId);
        Task UpdateQuantityAsync(int itemId, int quantity);
        Task RemoveItemAsync(int itemId);
    }
}
