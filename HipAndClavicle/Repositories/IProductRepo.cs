namespace HipAndClavicle.Repositories;

public interface IProductRepo
{
    public Task CreateProductAsync(Product product);
    public Task DeleteProductAsync(Product product);
    public Task<List<Product>> GetAvailableProductsAsync();
    public Task<Product> GetProductByIdAsync(int id);
    public Task UpdateProductAsync(Product product);
}
