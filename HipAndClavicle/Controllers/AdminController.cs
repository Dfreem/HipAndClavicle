﻿namespace HipAndClavicle.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAdminRepo _adminRepo;
    private readonly INotyfService _toast;
    IProductRepo _productRepo;

    public AdminController(IServiceProvider services)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _adminRepo = services.GetRequiredService<IAdminRepo>();
        _productRepo = services.GetRequiredService<IProductRepo>();
        _toast = services.GetRequiredService<INotyfService>();

    }

    public async Task<IActionResult> CurrentOrders()
    {
        // since the entire class is restricted to Admin Only,
        // no need to check for admin role in the controller methods.
        var admin = await _userManager.FindByNameAsync(User.Identity!.Name!);
        MerchantVM mvm = new() { Admin = admin!};

        // TODO turn these enums into binary options in order to select multpile
        mvm.CurrentOrders = await _adminRepo.GetAdminOrdersAsync(OrderStatus.Paid);
        //var lates = await _adminRepo.GetAdminOrdersAsync(OrderStatus.Late);
        //var shipped = await _adminRepo.GetAdminOrdersAsync(OrderStatus.Shipped);

        //if (lates is not null) mvm.CurrentOrders.AddRange(lates);
        //if (shipped is not null) mvm.CurrentOrders.AddRange(shipped);
        return View(mvm);
    }

    public async Task<IActionResult> Products()
    {
        var admin = await _userManager.FindByNameAsync(User!.Identity!.Name!);
        var products = await _productRepo.GetAvailableProductsAsync();

        ViewBag.Familes = await _productRepo.GetAllColorFamiliesAsync();
        var colors = await _productRepo.GetNamedColorsAsync();
      
        MerchantVM mvm = new()
        {
            Admin = admin!,
            Products = products,
            
        };
        return View(mvm);
    }
}
