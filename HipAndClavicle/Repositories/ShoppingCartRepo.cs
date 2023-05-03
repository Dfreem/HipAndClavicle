using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HipAndClavicle.Data;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HipAndClavicle.Repositories
{
    public class ShoppingCartRepo : IShoppingCartRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ShoppingCartRepo(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) 
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId)
        {
         
            // Load the cart from the database
            var shoppingCart = await _context.ShoppingCarts
                .Include(cart => cart.ShoppingCartItems)
                .ThenInclude(item => item.ListingItem)
                .FirstOrDefaultAsync(cart => cart.CartId == cartId);

            if (shoppingCart != null)
            {
                return shoppingCart;
            }
            else
            {
                if (ownerId != null)
                {
                    var owner = await _userManager.FindByIdAsync(ownerId);
                    shoppingCart = new ShoppingCart { CartId = cartId, Owner = owner };
                    _context.ShoppingCarts.Add(shoppingCart);
                    await _context.SaveChangesAsync();
                    return shoppingCart;
                }
                else
                {
                    return null;
                }
            }

            // If the cart doesn't exist, create a new one with the given cart ID
            shoppingCart = new ShoppingCart
            {
                CartId = cartId
            };
            _context.ShoppingCarts.Add(shoppingCart);
            await _context.SaveChangesAsync();

            return shoppingCart;
        }
       
        public async Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync(string cartId)
        {
            ShoppingCart shoppingCart = await _context.ShoppingCarts.Include(s => s.ShoppingCartItems)
            .ThenInclude(s => s.ListingItem)
            .FirstOrDefaultAsync(s => s.CartId == cartId && s.Owner.Id == null);

            if (shoppingCart != null)
            {
                return shoppingCart.ShoppingCartItems;
            }
            else
            {
                return new List<ShoppingCartItem>();
            }
        }

        public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
        {
            // Check if the listing is already in the cart
            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.ShoppingCartId == item.ShoppingCartId && i.ListingItem.ListingId == item.ListingItem.ListingId);

            if (existingItem == null)
            {
                // Add the listing to the cart
                await _context.ShoppingCartItems.AddAsync(item);
            }
            else
            {
                // Increment the quantity of the listing in the cart
                existingItem.Quantity += item.Quantity;
            }

            await _context.SaveChangesAsync();
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
