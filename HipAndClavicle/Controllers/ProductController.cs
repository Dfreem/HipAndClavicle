

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
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> EditProduct(int productId)
    //{
    //    ViewBag.Familes = await _productRepo.GetAllColorFamiliesAsync();
    //    var colors = await _productRepo.GetNamedColorsAsync();
    //    var toEdit = await _productRepo.GetProductByIdAsync(productId);
    //    ProductVM editProduct = new() { Edit = toEdit, NamedColors = colors };

    //    return View("Admin/Products", editProduct);
    //}

    [HttpPost]
    public async Task<IActionResult> EditProduct(Product product)
    {

        Product edit = await _productRepo.GetProductByIdAsync(product.ProductId);

        if (product.TempFile is not null)
        {
            edit.ProductImage = await ExtractImageAsync(product.TempFile);

        }
        if (product.Name is not null)
        {
            edit.Name = product.Name;
        }
        if(product.ColorFamilies is not null)
        {
            // TODO changed this to either add or remove
            edit.ColorFamilies = product.ColorFamilies;
        }
        await _productRepo.UpdateProductAsync(edit);
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
    public async Task<IActionResult> AddProduct([Bind("NewSize, SetSizes, Category, NewColor, ImageFile, QuantityOnHand, Edit, NewProduct ")] ProductVM pvm)
    {

        // check if Product is valid before trying to save
        if (ModelState.GetFieldValidationState("Edit") == ModelValidationState.Invalid)
        {
            // error if not valid
            ModelState.AddModelError("Edit", ModelState.ValidationState.ToDescriptionString());
            _toast.Error("somethihg went wrong while trying to save" + "\n" + ModelState.GetFieldValidationState("Edit").ToDescriptionString());
            return View(pvm);
        }
        // check if image is valid before trying to save
        if (pvm.ImageFile is not null)
        {
            pvm.Edit!.ProductImage = await ExtractImageAsync(pvm.ImageFile);
        }
        // check if color is valid before trying to save
        if (pvm.NewColor.HexValue is not null)
        {
            pvm.Edit!.AvailableColors.Add(pvm.NewColor);
        }
        // check if size is valid before trying to save
        if (pvm.NewSize.Size > 0)
        {
            pvm.Edit!.SetSizes.Add(pvm.NewSize);
        }
        await _productRepo.CreateProductAsync(pvm.Edit!);
        _toast.Success("Successfully created new product");
        return View(pvm);
    }

    /// <summary>
    /// Given an IFormFile with data from an input tag, extract the image data and return an Image object
    /// </summary>
    /// <param name="imageFile"><see cref="IFormFile"/> holding raw image file</param>
    /// <param name="width">the desired width of the resulting image. Only the width is needed so that aspect ratio is preserved</param>
    /// <returns>an <see cref="Image"/> Object containing the byte array data for the image. This is the format that should be saved in the database.</returns>
    public async Task<Image> ExtractImageAsync(IFormFile imageFile, int width = 100)
    {
        // memory stream to hold the image data
        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);
            // create a new image object with the data from the memory stream
            return new Image()
            {
                ImageData = memoryStream.ToArray(),
                Width = width
            };

        }
    }

    /// <summary>
    /// Delete a product from the database. This method should be called from a View.
    /// </summary>
    /// <param name="productId">The id of the product to delete.</param>
    /// <returns></returns>
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var toDelete = await _productRepo.GetProductByIdAsync(productId);
        await _productRepo.DeleteProductAsync(toDelete);
        return RedirectToAction("Products", "Admin");
    }
}