using System.Threading.Tasks;
using HipAndClavicle.Models;

namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepo
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId);
        //Task<ShoppingCart> GetShoppingCartByUser(string userId);
        Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync(string cartId);
        Task AddShoppingCartItemAsync(ShoppingCartItem item);
        //Task UpdateQuantityAsync(int itemId, int quantity);
        //Task RemoveItemAsync(int itemId);
    }
}
