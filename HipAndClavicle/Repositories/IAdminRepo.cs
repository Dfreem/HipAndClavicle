
namespace HipAndClavicle.Data
{
    public interface IAdminRepo
    {
        public Task CreateOrderAsync(Order order);
        public Task CreateProductAsync(Product product);
        public Task DeleteOrderAsync(Order order);
        public Task DeleteProductAsync(Product product);
        public Task<List<Order>> GetAdminCurrentOrdersAsync();
        public Task<List<OrderItem>> GetOrderItemsAsync();
        public Task<List<Product>> GetAvailableProductsAsync();
        public Task<Order?> GetOrderById(int id);
        public Task<Product> GetProductByIdAsync(int id);
        public Task<List<Color>> GetNamedColorsAsync();
        public Task<List<SetSize>> GetSetSizesAsync();
        public Task UpdateOrderAsync(Order order);
        public Task UpdateProductAsync(Product product);
        Task SaveImageAsync(Image fromUpload);
    }
}