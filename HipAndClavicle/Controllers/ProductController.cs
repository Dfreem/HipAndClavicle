

using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        var colors = await _productRepo.GetNamedColorsAsync();
        var toEdit = await _productRepo.GetProductByIdAsync(productId);
        ProductVM editProduct = new() { Edit = toEdit, NamedColors = colors };

        return View(editProduct);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(ProductVM product)
    {
        if (ModelState.GetValidationState("Edit.ProductId") == ModelValidationState.Invalid)
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
        setSizes = setSizes.Distinct().ToList();
        var colorFams = await _productRepo.GetAllColorFamiliesAsync();
        ProductVM product = new()
        {
            NamedColors = colorOptions,
            SetSizes = setSizes,
            Families = colorFams,
            Edit = new()
        };
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([Bind("NewSize, SetSizes, Category, NewColor, ImageFile, QuantityOnHand, Edit, Name, NewProduct ")] ProductVM pvm)
    {
        if (ModelState.GetFieldValidationState("Edit") == ModelValidationState.Invalid)
        {
            ModelState.AddModelError("Edit", ModelState.ValidationState.ToDescriptionString());
            _toast.Error("somethihg went wrong while trying to save" + "\n" + ModelState.GetFieldValidationState("Edit").ToDescriptionString());
            return View(pvm);
        }
        if (pvm.ImageFile is not null)
        {
            pvm.Edit!.ProductImage = await ExtractImageAsync(pvm.ImageFile);
        }
        if (pvm.NewColor.HexValue is not null)
        {
            pvm.Edit!.AvailableColors.Add(pvm.NewColor);
        }
        if (pvm.NewSize.Size > 0)
        {
            pvm.Edit!.SetSizes.Add(pvm.NewSize);
        }
        await _productRepo.CreateProductAsync(pvm.Edit!);
        _toast.Success("Successfully created new product");
        return View(pvm);
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
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var toDelete = await _productRepo.GetProductByIdAsync(productId);
        await _productRepo.DeleteProductAsync(toDelete);
        return RedirectToAction("Products", "Admin");
    }
}