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


    public async Task<List<Color>> GetNamedColorsAsync()
    {
        return await _context.NamedColors.ToListAsync();
    }

    public async Task<List<SetSize>> GetSetSizesAsync()
    {
        return await _context.SetSizes.ToListAsync();
    }

    public async Task AddNewSizeAsync(int size)
    {
        if (!_context.SetSizes.Any(s => s.Size == size))
        {
            SetSize newSize = new() { Size = size };
            _context.SetSizes.Add(newSize);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveImageAsync(Image fromUpload)
    {
        await _context.Images.AddAsync(fromUpload);
        await _context.SaveChangesAsync();
    }
    public async Task<List<ColorFamily>> GetAllColorFamiliesAsync()
    {
        return await _context.ColorFamilies.Include(f => f.Color)
            .ToListAsync();
    }


}
