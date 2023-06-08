
namespace HipAndClavicle.Repositories
{
    public interface IShoppingCartRepo
    {
        Task<ShoppingCart> GetShoppingCartByOwnerId(string ownerId);
        //Task <ShoppingCartItem> GetOrderItemByIdAsync(int id);
        Task AddShoppingCartItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task UpdateShoppingCartAsync(ShoppingCart sc);
        Task RemoveItemAsync(ShoppingCartItem item);
        Task ClearShoppingCartAsync(string cartId, string ownerId);
    }
}
