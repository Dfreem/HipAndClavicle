
namespace HipAndClavicle.Repositories;


public class OrderRepo : IOrderRepo
{
    readonly ApplicationDbContext _context;

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
