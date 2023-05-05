
namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepo
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId);
        Task<List<ShoppingCartItemViewModel>> GetShoppingCartItemsAsync(IEnumerable<ShoppingCartItem> items);
        Task <ShoppingCartItem> GetCartItem(int id);
        Task AddShoppingCartItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task RemoveItemAsync(ShoppingCartItem item);
    }
}
