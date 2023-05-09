
namespace HipAndClavicle.Controllers;
[Authorize(Roles = "Admin")]
public class ShipController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IShippingRepo _repo;
    private readonly INotyfService _toast;
    private readonly string _shipEngineKey;


    public ShipController(IServiceProvider services, IConfiguration config)
    {
        _services = services;
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _userManager = _signInManager.UserManager;
        _repo = services.GetRequiredService<IShippingRepo>();
        _toast = services.GetRequiredService<INotyfService>();

        // Ship Engine Key for Shipments
        _shipEngineKey = config["ShipEngine"]!;
        
    }

    public async Task<IActionResult> Ship(int orderId)
    {
        var merchant = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity!.Name!);
        merchant!.Address = await _repo.FindUserAddress(merchant);
        var order = await _repo.GetOrderByIdAsync(orderId);


        ShippingVM shippingVM = new()
        {
            OrderToShip = order,
            Customer = order.Purchaser,
            Merchant = merchant!,
        };
        return View(shippingVM);
    }

    // POST: Ship/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ship(ShippingVM svm)
    {
        svm.OrderToShip = await _repo.GetOrderByIdAsync(svm.OrderToShip.OrderId);
        if (svm.OrderToShip is not null)
        {
            var merchant = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity!.Name!);

            if (merchant!.Address is null)
            {
                MerchantVM mvm = new()
                {
                    Admin = merchant,
                    FromAddress = new()
                };
                return View("NoMerchantAddressError", mvm);
            }
           

            return RedirectToAction(nameof(ViewLabel));

        }

        _toast.Error("Could not find order in system");
        return View(svm);
    }

    private IActionResult ViewLabel()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Shipping API

       
    #endregion

    #region Utility

    public async Task<IActionResult> ChangeMerchantAddress(ShippingVM svm)
    {
        await _userManager.UpdateAsync(svm.Merchant);
        return RedirectToAction("Ship");
    }

    #endregion

}
