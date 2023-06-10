namespace HipAndClavicle.Repositories;

public class CustRepo : ICustRepo
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;

    public CustRepo(IServiceProvider services, ApplicationDbContext context)
    {
        _services = services;
        _context = context;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
    }
    #region GetAll
    public async Task<List<Listing>> GetAllListingsAsync()
    {
        var listings = await _context.Listings
            .Include(l => l.Colors)
            .Include(l => l.SingleImage)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.AvailableColors)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.ProductImage)
            .Include(l => l.ListingColorJTs)
            .ThenInclude(lc  => lc.ListingColor)
            .ToListAsync();

        return listings;
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .ToListAsync();
        return products;
    }

    public async Task<List<Color>> GetAllColorsAsync()
    {
        var colors = await _context.NamedColors
            .ToListAsync();
        return colors;
    }

    #endregion

    #region GetSpecific
    public async Task<Listing> GetListingByIdAsync(int listingId)
    {
        var listing = await _context.Listings
            .Include(l => l.Colors)
            .Include(l => l.SingleImage)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.AvailableColors)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.ProductImage)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.SetSizes)
            .Include(l => l.ListingColorJTs)
            .ThenInclude(lc => lc.ListingColor)
            .Include(l => l.ListingProduct)
            .ThenInclude(p => p.Reviews)
            .ThenInclude(r => r.Reviewer)
            .Where(l => l.ListingId == listingId).FirstOrDefaultAsync();
        return listing;
    }
    public async Task<List<Color>> GetColorsByColorFamilyNameAsync(string colorFamilyName)
    {
        var colors = await _context.NamedColors
            .Include(c => c.ColorFamilies)
            .Where(c => c.ColorFamilies.Any(cf => cf.ColorFamilyName == colorFamilyName))
            .ToListAsync();

        return colors;
    }

    public async Task<List<Listing>> GetListingsByColorFamilyAsync(string colorFamilyName)
    {
        var colors = await GetColorsByColorFamilyNameAsync(colorFamilyName);   
        var listings = await _context.Listings
            .Include(l => l.Colors)
            .Include(l => l.ListingProduct)
            .Include(l => l.SingleImage)
            .Where(l => l.Colors.Any(c =>  colors.Contains(c))).ToListAsync();
        return listings;
    }

    public async Task<Color> GetColorByIdAsync(int searchColorId)
    {
        var color = await _context.NamedColors.Where(c => c.ColorId == searchColorId).FirstOrDefaultAsync();
        return color;
    }
    public async Task<Product> GetProductByIdAsync(int productId)
    {
        var product = await _context.Products.Where(p => p.ProductId == productId).FirstOrDefaultAsync();
        return product;
    }

    public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
    {
        //var orders = await _context.Orders
        //    .Include(o => o.Items)
        //    .ThenInclude(i => i.Item)
        //    .Include(o => o.Items)
        //    .ThenInclude(c => c.ItemColors)
        //    //.Include(o => o.Items)
        //    //.ThenInclude(i => i.TheListing)
        //    //.ThenInclude(tl => tl.Colors)
        //    .Where(o => o.PurchaserId == customerId).ToListAsync();

        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .ThenInclude(m => m.AvailableColors)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .ThenInclude(m => m.ProductImage)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .Include(o => o.Items)
            .ThenInclude(i => i.SetSize)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.ItemColors)
            .Where(o => o.PurchaserId == customerId).ToListAsync();
        return orders;
    }

    public async Task<Order> GetOrderById( int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .Where(o => o.OrderId == orderId).FirstOrDefaultAsync();
        return order;
    }

    public async Task<ShoppingCart> GetCartByCustId(string custId)
    {
        var cart = await _context.ShoppingCarts
            .Include(c => c.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.Colors)
            .Include(c => c.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.ListingProduct)
            .ThenInclude(p => p.AvailableColors)
            .Include(c => c.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.SingleImage).FirstAsync(c => c.CartId == custId);

        return cart;
    }

    public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId)
    {

        // Load the cart from the database
        var shoppingCart = await _context.ShoppingCarts
            .Include(cart => cart.ShoppingCartItems)
            .ThenInclude(item => item.ListingItem)
            .ThenInclude(li => li.ListingProduct)
            .Include(cart => cart.ShoppingCartItems)
            .ThenInclude(item => item.ListingItem)
            .ThenInclude(li => li.Colors)
            .FirstOrDefaultAsync(cart => cart.CartId == cartId);

        return shoppingCart;
    }

    #endregion

    #region MakeUpdates
    public async Task AddColorFamilyAsync(ColorFamily colorFamily)
    {
        await _context.ColorFamilies.AddAsync(colorFamily);
        await _context.SaveChangesAsync();
    }

    public async Task AddListingAsync(Listing listing)
    {
        await _context.Listings.AddAsync(listing);
        await _context.SaveChangesAsync();
    }

    public async Task AddListingImageAsync(Image image)
    {
        await _context.Images.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task AddColorToListing(Listing listing, Color color)
    {
        var listingColorAssociation = new ListingColorJT()
        {
            Listing = listing,
            ListingColor = color
        };
        await _context.ListingColorsJT.AddAsync(listingColorAssociation);
    }

    public async Task AddReviewAsync(CustReviewVM crVM)
    {
        //Review newReview = new Review()
        //{
        //    Reviewer = crVM.Reviewer,
        //    ReviewedProductId = crVM.ProductId,
        //    Message = crVM.Review.Message,
        //};
        var product = crVM.Product;
        product.Reviews.Add(crVM.Review);
        await _context.Reviews.AddAsync(crVM.Review);
        await _context.SaveChangesAsync();
    }

    public async Task AddOrderByCheckoutVmAsync(CheckoutVM checkoutVm)
    {
        var order = checkoutVm.Order;
        //order.Status = OrderStatus.Paid;
        //order.DateOrdered = DateTime.Now;

        //order.Address.AddressLine1 = checkoutVm.Street;
        //order.Address.CityTown = checkoutVm.City;
        //order.Address.StateAbr = checkoutVm.State;
        //order.Address.PostalCode = checkoutVm.Zip;
        //order.Address.Name = checkoutVm.Name;

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task ClearShoppingCartAsync(string cartId)
    {
        ShoppingCart shoppingCart = await GetOrCreateShoppingCartAsync(cartId);

        _context.ShoppingCartItems.RemoveRange(shoppingCart.ShoppingCartItems);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Checks
    public async Task<bool> UserPurchasedProduct(int productId, AppUser currentUser)
    {
        //var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        var purchacedOrders = _context.Orders
            .Include(o => o.Purchaser)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .Where(o => o.Purchaser == currentUser)
            .ToListAsync();

        var currentProduct = _context.Products
            .Where(p => p.ProductId == productId)
            .FirstOrDefaultAsync();

        List<Product> products = new List<Product>();

        foreach (var order in await purchacedOrders)
        {
            foreach (var orderItem in order.Items)
            {
                if (orderItem.Item.ProductId == productId)
                {
                    return true;
                }
            }
        }

        

        return false;
    }

    #endregion Checks
}
