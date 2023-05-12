﻿
namespace HipAndClavicle;

public class ProductController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAdminRepo _adminRepo;
    private readonly IProductRepo _productRepo;
    private readonly INotyfService _toast;

    public ProductController(IServiceProvider services)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _adminRepo = services.GetRequiredService<IAdminRepo>();
        _productRepo = services.GetRequiredService<IProductRepo>();
        _toast = services.GetRequiredService<INotyfService>();
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditProduct(int productId)
    {
        ViewBag.Familes = await _productRepo.GetAllColorFamiliesAsync();
        ViewBag.NamedColors = await _productRepo.GetNamedColorsAsync();
        var toEdit = await _productRepo.GetProductByIdAsync(productId);
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

            await _productRepo.UpdateProductAsync(product.Edit);

        }
        return RedirectToAction("Products", "Admin");
    }


    public async Task<IActionResult> AddProduct()
    {
        var colorOptions = await _productRepo.GetNamedColorsAsync();
        var setSizes = await _productRepo.GetSetSizesAsync();
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
        Product newProduct = (Product)product;
        if (product.ImageFile is not null)
        {
            
            newProduct.ProductImage = await ExtractImageAsync(product.ImageFile);
            await _productRepo.SaveImageAsync(newProduct.ProductImage);

        }

        await _productRepo.CreateProductAsync(newProduct);
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
        Product toDelete = await _productRepo.GetProductByIdAsync(id);
        await _productRepo.DeleteProductAsync(toDelete);
        return RedirectToAction("Products");
    }
}