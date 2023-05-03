
namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepo
    {
        Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId);
        Task<List<ShoppingCartItemViewModel>> GetShoppingCartItemsAsync(IEnumerable<ShoppingCartItem> items);
        Task AddShoppingCartItemAsync(ShoppingCartItem item);
    }
}
