
namespace HipAndClavicle.Repositories
{
    public class ShoppingCartRepo : IShoppingCartRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ShoppingCartRepo(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ShoppingCart> GetShoppingCartByOwnerId(string? ownerId)
        {
            if (ownerId == null)
            {
                // Load the cart from the database
                var shoppingCart = await _context.ShoppingCarts
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.SetSize)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.ItemColors)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.Item)
                    .Include(i => i.Owner)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.Item)
                    .ThenInclude(i => i.ProductImage)
                    .FirstAsync(cart => cart.OwnerId == ownerId);
                return shoppingCart;
            }
            return new ShoppingCart();
        }

        //public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
        //{
            //// Check if the listing is already in the cart
            //var existingItem = await _context.ShoppingCartItems
            //    .FirstOrDefaultAsync(i => i.ShoppingCartId == item.ShoppingCartId && i.ListingItem.ListingId == item.ListingItem.ListingId);

            //if (existingItem == null)
            //{
            //    // Add the listing to the cart
            //    await _context.ShoppingCartItems.AddAsync(item);
            //}
            //else
            //{
            //    // Increment the quantity of the listing in the cart
            //    existingItem.Quantity += item.Quantity;
            //}

            //await _context.SaveChangesAsync();
        //}

        public async Task UpdateShoppingCartAsync(ShoppingCart sc)
        {
            _context.ShoppingCarts.Update(sc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(ShoppingCartItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearShoppingCartAsync(string cartId, string ownerId)
        {
            ShoppingCart shoppingCart = await GetShoppingCartByOwnerId(ownerId);

            _context.OrderItems.RemoveRange(shoppingCart.Items);
            await _context.SaveChangesAsync();
        }
    }
}
