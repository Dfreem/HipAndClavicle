namespace HipAndClavicle.Repositories;

public class ShippingRepo : IShippingRepo
{
    ApplicationDbContext _context;
    public ShippingRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Order

    public async Task<Order> GetOrderByIdAsync(int orderId) =>
        await _context.Orders
            .Include(o => o.Purchaser)
            .Include(o => o.Address)
            .Include(o => o.Items)
            .ThenInclude(i => i.Item)
            .Include(o => o.Address)
            .FirstAsync(o => o.OrderId.Equals(orderId));

    public async Task<List<OrderItem>> GetItemsToShipAsync(int OrderId)
    {
        return await _context.OrderItems
            .Include(i => i.ItemColors)
            .Include(i => i.Item)
            .Include(i => i.SetSize)
            .Where(i => i.OrderId == OrderId).ToListAsync();
    }

    #endregion

    #region Shipping
    public async Task CreateShipment(Ship shipment)
    {
        await _context.Shipping.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateShipment(Ship shipment)
    {
        _context.Shipping.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task<Ship> GetShipmentByIdAsync(int id) =>
        await _context.Shipping
            .Include(s => s.Order)
            .Include(s => s.Order.Address).FirstAsync(s => s.ShipId == id);

    public async Task<ShippingAddress> FindUserAddress(AppUser user)
    {
        return await _context.Addresses.FirstAsync(a => a.ShippingAddressId.Equals(user.ShippingAddressId));
    }

    #endregion
}
