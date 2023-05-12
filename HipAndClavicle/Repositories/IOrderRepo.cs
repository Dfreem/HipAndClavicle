namespace HipAndClavicle.Repositories
{
    public interface IOrderRepo
    {
        Task CreateOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderAsync(Order order);
        Task DeleteOrderItemAsync(OrderItem orderItem);
        Task<OrderItem> GetOrderItemById(int id);
        Task<List<OrderItem>> GetOrderItemsAsync();
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderItemAsync(OrderItem orderItem);
    }
}