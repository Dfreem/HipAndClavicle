
using HipAndClavicle;
using HipAndClavicle.Models;
using HipAndClavicle.Repositories;

namespace HIPNunitTests.Fakes
{
    public class FakeProductRepo : IProductRepo
    {
        private List<Product> _products;
        private List<Color> _namedColors;
        private List<SetSize> _setSizes;
        private List<ColorFamily> _colorFamilies;

        public FakeProductRepo()
        {
            _products = new List<Product>();
            _namedColors = new List<Color>();
            _setSizes = new List<SetSize>();
            _colorFamilies = new List<ColorFamily>();
        }

        public Task CreateProductAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            return Task.FromResult(product);
        }

        public Task<List<Product>> GetAvailableProductsAsync()
        {
            return Task.FromResult(_products);
        }

        public Task UpdateProductAsync(Product product)
        {
            // No action needed for the fake repository.
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(Product product)
        {
            _products.Remove(product);
            return Task.CompletedTask;
        }

        public Task<List<Color>> GetNamedColorsAsync()
        {
            return Task.FromResult(_namedColors);
        }

        public Task<List<SetSize>> GetSetSizesAsync()
        {
            return Task.FromResult(_setSizes);
        }

        public Task AddNewSizeAsync(int size)
        {
            var newSize = new SetSize { Size = size };
            _setSizes.Add(newSize);
            return Task.CompletedTask;
        }

        public Task SaveImageAsync(Image fromUpload)
        {
            // No action needed for the fake repository.
            return Task.CompletedTask;
        }

        public Task<List<ColorFamily>> GetAllColorFamiliesAsync()
        {
            return Task.FromResult(_colorFamilies);
        }

        public Task AddNewColorAsync(Color newColor)
        {
            _namedColors.Add(newColor);
            return Task.CompletedTask;
        }
    }
}
