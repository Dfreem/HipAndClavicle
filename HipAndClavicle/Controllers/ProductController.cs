
using NUnit.Framework;
using shippingapi.Model;

namespace HipAndClavicle;

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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditProduct(int productId)
    {
        ViewBag.Familes = await _repo.GetAllColorFamiliesAsync();
        ViewBag.NamedColors = await _repo.GetNamedColorsAsync();
        var toEdit = await _repo.GetProductByIdAsync(productId);
        ProductVM editProduct = new() { Edit = toEdit };
        return View(editProduct);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(ProductVM product)
    {
        if (!ModelState.IsValid)
        {
            // TODO better error message
            _toast.Error("Something went wrong");
            return View(product);
        }
        
        if (product.ImageFile is not null)
        {
            product.Edit!.ProductImage = await ExtractImageAsync(product.ImageFile);

            await _repo.UpdateProductAsync(product.Edit);

        }
        return RedirectToAction("Products", "Admin");
    }


    public async Task<IActionResult> AddProduct()
    {
        var colorOptions = await _repo.GetNamedColorsAsync();
        var setSizes = await _repo.GetSetSizesAsync();
        ProductVM product = new()
        {
            NamedColors = colorOptions,
            SetSizes = setSizes
        };
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductVM product)
    {
        if (!ModelState.IsValid)
        {
            _toast.Error("ModelState is not valid");
            return View(product);
        }
        Image fromUpload = await ExtractImageAsync(product.ImageFile);
        await _repo.SaveImageAsync(fromUpload);
        await _repo.CreateProductAsync((Product)product);
        _toast.Success("Successfully created new product");
        return RedirectToAction("Products");
    }

    public async Task<Image> ExtractImageAsync(IFormFile imageFile, int width = 100)
    {
        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);
            return new Image()
            {
                ImageData = memoryStream.ToArray(),
                Width = width
            };

        }
    }
    public async Task<IActionResult> DeleteProductAsync(int id)
    {
        Product toDelete = await _repo.GetProductByIdAsync(id);
        await _repo.DeleteProductAsync(toDelete);
        return RedirectToAction("Products");
    }
}