namespace HipAndClavicle.Controllers;

// Restrict Access of Controller and Views to only users with the "Admin" role
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
        var adminSettings = await _adminRepo.GetSettingsForUserAsync(admin!.Id);
        if (adminSettings is default(UserSettings))
        {
            adminSettings!.User = admin;
            await _adminRepo.UpdateUserSettingsAsync(adminSettings);
        }
        MerchantVM mvm = new() 
        { 
            Admin = admin!,
            Settings = adminSettings!
        };
        mvm.CurrentOrders = await _adminRepo.GetAdminOrdersAsync(OrderStatus.Paid | OrderStatus.ReadyToShip | OrderStatus.Late);
        mvm.ShippedOrders = await _adminRepo.GetAdminOrdersAsync(OrderStatus.Shipped);

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
