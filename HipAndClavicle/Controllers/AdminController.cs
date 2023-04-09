﻿namespace HipAndClavicle.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHipRepo _repo;

    public AdminController(IServiceProvider services)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _repo = services.GetRequiredService<IHipRepo>();
    }
    public async Task<IActionResult> Index()
    {
        // since the entire class is restricted to Admin Only,
        // no need to check for admin role in the controller methods.
        var admin = await _userManager.FindByNameAsync(User.Identity!.Name!);
        MerchantVM mvm = new(admin!)
        {
            CurrentOrders = await _repo.GetAdminCurrentOrdersAsync()
        };
        return View(mvm);
    }
}

