
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
                return await _context.ShoppingCarts
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.SetSize)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.ItemColors)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.Item)
                    .Include(i => i.Owner)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.ListingItem)
                    .Include(cart => cart.Items)
                    .ThenInclude(items => items.Item)
                    .ThenInclude(i => i.ProductImage)
                    .FirstAsync(cart => cart.OwnerId == ownerId);            
        }



        public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
        {
            // Check if the listing is already in the cart
            var owner = await _userManager.FindByIdAsync(_signInManager.Context.User.Identity!.Name!);
            ShoppingCart cart;
            if (owner is not null)
            {
                cart = await _context.ShoppingCarts.FindAsync(owner.Cart.ShoppingCartId) ??
                    new ShoppingCart()
                    {
                        Owner = owner,
                    };
                cart!.Items.Add(item);
            }
            else
            {
                cart = _context.ShoppingCarts.FirstOrDefault(cart => cart.Owner!.UserName == "DEFAULT") ??
                    new()
                    {
                        Owner = new AppUser()
                        {
                            UserName = "DEFAULT"
                        }
                    };
                cart!.Items.Add(item);
            }
            _context.ShoppingCarts.Update(cart);
            await _context.SaveChangesAsync();
        }

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
            var owner = await _userManager.FindByIdAsync(_signInManager.Context.User.Identity!.Name!);
            var cart = await _context.ShoppingCarts.Include(cart => cart.Items).FirstAsync(cart => cart.Items.Contains(item));
            cart!.Items.Remove(item);
            
            _context.ShoppingCarts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task ClearShoppingCartAsync(string cartId, string ownerId)
        {
            ShoppingCart shoppingCart = await GetShoppingCartByOwnerId(ownerId);

            _context.OrderItems.RemoveRange(shoppingCart.Items);
            await _context.SaveChangesAsync();
        }
        public async Task<OrderItem> GetOrderItemByIdAsync(int ItemId)
        {
            return await _context.OrderItems.FirstAsync(item => item.OrderItemId == ItemId);
        }

        public async Task UpdateItemAsync(OrderItem item)
        {
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();
            
        }

        public async Task<ShoppingCartItem> GetShoppingCartItemByIdAsync(int Id)
        {
            return await _context.ShoppingCartItems.FindAsync(Id)??new();
            
        }
    }
}
