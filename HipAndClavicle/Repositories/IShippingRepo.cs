namespace HipAndClavicle.Repositories
{
    public interface IShippingRepo
    {
        public Task CreateShipment(Ship shipment);
        public Task<List<OrderItem>> GetItemsToShipAsync(int OrderId);
        public Task<Ship> GetShipmentByIdAsync(int id);
        public Task UpdateShipment(Ship shipment);
        public Task<Order> GetOrderByIdAsync(int orderId);
        public Task<ShippingAddress> FindUserAddress(AppUser user);

    }
}