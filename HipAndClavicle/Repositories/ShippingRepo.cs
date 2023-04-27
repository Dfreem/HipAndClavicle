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
            .FirstAsync(o => o.OrderId.Equals(orderId));

    public async Task<List<OrderItem>> GetItemsToShipAsync(int OrderId)
    {
        return await _context.OrderItems
            .Include(i => i.ItemColor)
            .Include(i => i.Item)
            .Include(i => i.SetSize)
            .Where(i => i.OrderId == OrderId).ToListAsync();
    }

    #endregion

    #region Shipping
    public async Task CreateShippment(Ship shipment)
    {
        await _context.Shipping.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateShippment(Ship shipment)
    {
        _context.Shipping.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task<Ship> GetShipmentByIdAsync(int id) =>
        await _context.Shipping
            .Include(s => s.Order)
            .Include(s => s.Order.Address).FirstAsync(s => s.ShipId == id);
    #endregion
}
