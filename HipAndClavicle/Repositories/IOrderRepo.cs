﻿namespace HipAndClavicle.Repositories
{
    public interface IOrderRepo
    {
        Task CreateOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderAsync(Order order);
        Task DeleteOrderItemAsync(OrderItem orderItem);
        Task<Order> GetOrderForPaymentAsync(int orderId);
        Task<List<OrderItem>> GetOrderItemsAsync();
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderItemAsync(OrderItem orderItem);
    }
}