namespace HipAndClavicle.Repositories;

public class OrderRepo : IOrderRepo
{
    readonly ApplicationDbContext _context;
    readonly AdminSettings _adminSettings;
    public OrderRepo(ApplicationDbContext context)
    {
        _context = context;
        //_adminSettings = _context.Settings.FirstAsync(s => s.)
    }

    public async Task CreateOrder(ShoppingCart cart)
    {
        Order order = new()
        {
            Purchaser = cart.Owner,
            Address = cart.Owner.Address!,
            DateOrdered = DateTime.Now,
        };
        foreach (var item in cart.ShoppingCartItems)
        {
            order.Items.Add(item.ToOrderItem(order));
        }
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<ShoppingCart> GetShoppingCartById(int id)
    {
        return await _context.ShoppingCarts
            .Include(sc => sc.Owner)
            .ThenInclude(o => o.Address)
            .Include(sc => sc.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.Colors)
            .Include(sc => sc.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.Images)
            .FirstAsync(sc => sc.Id == id);
    }

}
