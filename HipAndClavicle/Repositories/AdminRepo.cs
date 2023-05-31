
namespace HipAndClavicle.Repositories;

public class AdminRepo : IAdminRepo
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminRepo(IServiceProvider services, ApplicationDbContext context)
    {
        _services = services;
        _context = context;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
    }

    /// <summary>
    /// Get all the orders stored in the database with the same <see cref="OrderStatus"/>
    /// </summary>
    /// <param name="status">the <see cref="OrderStatus"/> of the orders to retrieve</param>
    /// <returns>a <see cref="List\<Order>"/></returns>
    public async Task<List<Order>> GetAdminOrdersAsync(OrderStatus status)
    {
        var orders = await _context.Orders
            .Include(o => o.Purchaser)
            .Include(o => o.Address)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .ThenInclude(i => i.SetSizes)
            .Include(o => o.Items)
            .ThenInclude(i => i.ItemColors)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item.ProductImage)
            .Where(o => o.Status.Equals(status))
            .ToListAsync();
        return orders;
    }
}
