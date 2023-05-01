using System.Linq;
using System.Threading.Tasks;
using HipAndClavicle.Data;
using HipAndClavicle.Models;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string shoppingCartId)
        {
            var shoppingCart = await _context.ShoppingCarts
                .Include(sc => sc.ShoppingCartItems)
                .ThenInclude(sci => sci.Product)
                .FirstOrDefaultAsync(sc => sc.Owner.Id == shoppingCartId);

            // If the cart doesn't exist, create a new one for the user
            if (shoppingCart == null)
            {
                
                shoppingCart = new ShoppingCart { Owner = await _context.Users.FindAsync(shoppingCartId) };
                _context.ShoppingCarts.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }
            return shoppingCart;
        }

        public async Task<ShoppingCart> GetShoppingCartByUser(string userId)
        {
            return _context.ShoppingCarts
                .Include(sc => sc.ShoppingCartItems)
                    .ThenInclude(sci => sci.Product)
                .FirstOrDefault(sc => sc.Owner.Id == userId);
        }

        public async Task<List<ShoppingCartItem>> GetItemsAsync(string userId)
        {
            var cart = await GetOrCreateShoppingCartAsync(userId);
            return cart.ShoppingCartItems;
        }

        public async Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item, string userId)
        {
            // Check if the item already exists in the cart
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.ShoppingCart.Owner.Id == userId && i.Product.ProductId == item.Product.ProductId);

            if (existingItem != null)
            {
                // If the item exists, update the quantity and save changes
                existingItem.Quantity += item.Quantity;
                _context.Entry(existingItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return existingItem;
            }

            // If the item doesn't exist, add it to the cart
            var cart = await GetOrCreateShoppingCartAsync(userId);
            item.ShoppingCart = cart;
            _context.ShoppingCartItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateQuantityAsync(int itemId, int quantity)
        {
            var item = await _context.ShoppingCartItems.FindAsync(itemId);

            if (item != null)
            {
                item.Quantity = quantity;
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(int itemId)
        {
            var item = await _context.ShoppingCartItems.FindAsync(itemId);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
