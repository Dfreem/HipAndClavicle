
namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepo
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId);
        Task <ShoppingCartItem> GetOrderItemByIdAsync(int id);
        Task AddShoppingCartItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task RemoveItemAsync(ShoppingCartItem item);
        Task ClearShoppingCartAsync(string cartId, string ownerId);
    }
}
