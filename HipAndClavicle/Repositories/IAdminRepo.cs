namespace HipAndClavicle.Repositories;

public interface IAdminRepo
{
    public Task CreateProductAsync(Product product);
    public Task DeleteOrderAsync(Order order);
    public Task DeleteProductAsync(Product product);
    public Task<List<Order>> GetAdminCurrentOrdersAsync();
    public Task<List<Product>> GetAvailableProductsAsync();
    public Task<Product> GetProductByIdAsync(int id);
    public Task<List<Color>> GetNamedColorsAsync();
    public Task<List<SetSize>> GetSetSizesAsync();
    public Task UpdateOrderAsync(Order order);
    public Task UpdateProductAsync(Product product);
    public Task SaveImageAsync(Image fromUpload);
    Task<List<OrderItem>> GetOrderItemsAsync();
}