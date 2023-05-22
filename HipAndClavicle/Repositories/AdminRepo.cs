
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
    public async Task<List<Order>> GetAdminCurrentOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Purchaser)
            .Include(o => o.Address)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .ThenInclude(i => i.SetSizes)
            .Include(o => o.Items)
            .ThenInclude(i => i.ItemColors)
            .Where(o => !o.Status.Equals(OrderStatus.Shipped))
            .ToListAsync();
        return orders;
    }
}
