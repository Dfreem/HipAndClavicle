namespace HipAndClavicle.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAdminRepo _repo;
    private readonly INotyfService _toast;

    public AdminController(IServiceProvider services)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _repo = services.GetRequiredService<IAdminRepo>();
        _toast = services.GetRequiredService<INotyfService>();
    }

    public async Task<IActionResult> CurrentOrders()
    {
        // since the entire class is restricted to Admin Only,
        // no need to check for admin role in the controller methods.
        var admin = await _userManager.FindByNameAsync(User.Identity!.Name!);
        MerchantVM mvm = new(admin!);
        mvm.CurrentOrders = await _repo.GetAdminCurrentOrdersAsync();
        foreach (var order in mvm.CurrentOrders)
        {
            order.Items = await _repo.GetOrderItemsAsync();
        }

        return View(mvm);
    }

    public async Task<IActionResult> Products()
    {
        var admin = await _userManager.FindByNameAsync(User!.Identity!.Name!);
        var products = await _repo.GetAvailableProductsAsync();
        MerchantVM mvm = new()
        {
            Admin = admin!,
            Products = products
        };
        return View(mvm);
    }

    //public async Task<IActionResult> AddProduct()
    //{
    //    var colorOptions = await _repo.GetNamedColorsAsync();
    //    var setSizes = await _repo.GetSetSizesAsync();
    //    AddProductVM product = new()
    //    {
    //        NamedColors = colorOptions,
    //        SetSizes = setSizes
    //    };
    //    return View(product);
    //}

    //[HttpPost]
    //public async Task<IActionResult> AddProduct(AddProductVM product)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        _toast.Error("ModelState is not valid");
    //        return View(product);
    //    }

    //    using (var memoryStream = new MemoryStream())
    //    {
    //        await product.ImageFile.CopyToAsync(memoryStream);
    //        Image fromUpload = new()
    //        {
    //            ImageData = memoryStream.ToArray(),
    //            Width = 100
    //        };
    //        product.ProductImage = fromUpload;
    //        await _repo.SaveImageAsync(fromUpload);
    //        await _repo.CreateProductAsync((Product)product);
    //        _toast.Success("Successfully created new product");
    //        return RedirectToAction("Products");
    //    }
    //}

    //public async Task<IActionResult> DeleteProductAsync(int id)
    //{
    //    Product toDelete = await _repo.GetProductByIdAsync(id);
    //    await _repo.DeleteProductAsync(toDelete);
    //    return RedirectToAction("Products");
    //}

}
