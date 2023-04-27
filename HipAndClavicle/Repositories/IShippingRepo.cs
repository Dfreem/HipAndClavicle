namespace HipAndClavicle.Repositories
{
    public interface IShippingRepo
    {
        Task CreateShippment(Ship shipment);
        Task<List<OrderItem>> GetItemsToShipAsync(int OrderId);
        Task<Ship> GetShipmentByIdAsync(int id);
        Task UpdateShippment(Ship shipment);
        Task<Order> GetOrderByIdAsync(int orderId);
    }
}