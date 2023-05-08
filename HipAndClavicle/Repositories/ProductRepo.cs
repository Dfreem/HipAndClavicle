namespace HipAndClavicle.Repositories;

public class ProductRepo : IProductRepo
{

    ApplicationDbContext _context;
    public ProductRepo(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id) =>
        await _context.Products
            .Include(p => p.AvailableColors)
            .Include(p => p.Reviews)
            .Include(p => p.ProductImage)
            .FirstAsync(p => p.ProductId.Equals(id));

    public async Task<List<Product>> GetAvailableProductsAsync() =>
        await _context.Products
            .Include(p => p.AvailableColors)
            .Include(p => p.Reviews)
            .Include(p => p.ProductImage)
            .ToListAsync();

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

}
