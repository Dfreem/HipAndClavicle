namespace HipAndClavicle.Repositories;

public interface IProductRepo
{
    public Task CreateProductAsync(Product product);
    public Task DeleteProductAsync(Product product);
    public Task<List<Product>> GetAvailableProductsAsync();
    public Task<Product> GetProductByIdAsync(int id);
    public Task UpdateProductAsync(Product product);
    public Task SaveImageAsync(Image fromUpload);
    public Task<List<ColorFamily>> GetAllColorFamiliesAsync();
    public Task<List<Color>> GetNamedColorsAsync();
    public Task<List<SetSize>> GetSetSizesAsync();
    Task AddNewColorAsync(Color newColor);
}
