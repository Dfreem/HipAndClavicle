
using shippingapi.Model;

namespace HipAndClavicle
{
    public class ProductController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminRepo _repo;
        private readonly INotyfService _toast;

        public ProductController(IServiceProvider services)
        {
            _services = services;
            _userManager = services.GetRequiredService<UserManager<AppUser>>();
            _repo = services.GetRequiredService<IAdminRepo>();
            _toast = services.GetRequiredService<INotyfService>();
        }
        public async Task<IActionResult> EditProduct(int id)
        {
            var toEdit = await _repo.GetProductByIdAsync(id);
            return View(toEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            await _repo.UpdateProductAsync(product);
            return RedirectToAction("Products", "Admin");
        }


        public async Task<IActionResult> AddProduct()
        {
            var colorOptions = await _repo.GetNamedColorsAsync();
            var setSizes = await _repo.GetSetSizesAsync();
            AddProductVM product = new()
            {
                NamedColors = colorOptions,
                SetSizes = setSizes
            };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductVM product)
        {
            if (!ModelState.IsValid)
            {
                _toast.Error("ModelState is not valid");
                return View(product);
            }

            using (var memoryStream = new MemoryStream())
            {
                await product.ImageFile.CopyToAsync(memoryStream);
                Image fromUpload = new()
                {
                    ImageData = memoryStream.ToArray(),
                    Width = 100
                };
                product.ProductImage = fromUpload;
                await _repo.SaveImageAsync(fromUpload);
                await _repo.CreateProductAsync((Product)product);
                _toast.Success("Successfully created new product");
                return RedirectToAction("Products");
            }
        }

        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            Product toDelete = await _repo.GetProductByIdAsync(id);
            await _repo.DeleteProductAsync(toDelete);
            return RedirectToAction("Products");
        }
    }
}

