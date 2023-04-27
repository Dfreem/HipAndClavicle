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
                .ThenInclude(sci => sci.Listing)
                .FirstOrDefaultAsync(sc => sc.ShoppingCartId == shoppingCartId);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { ShoppingCartId = shoppingCartId };
                _context.ShoppingCarts.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }
            return shoppingCart;
        }

        public async Task<ShoppingCartItem> GetCartItemAsync(int cartItemId)
        {
            return await _context.ShoppingCartItems
                .Include(item => item.Listing)
                .FirstOrDefaultAsync(item => item.Id == cartItemId);
        }


        public async Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateItemAsync(ShoppingCartItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int itemId)
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
