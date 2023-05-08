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
}
