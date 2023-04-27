
using HipAndClavicle.Repositories;

namespace HipAndClavicle.Controllers;

public class ShipController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IShippingRepo _repo;
    private readonly INotyfService _toast;

    public ShipController(IServiceProvider services)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _repo = services.GetRequiredService<IShippingRepo>();
        _toast = services.GetRequiredService<INotyfService>();
    }

    public IActionResult Create(Order? toShip)
    {
        if (toShip == null) { toShip = new Order(); }
        
        return View(toShip);
    }

    // POST: Ship/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int orderId)
    {
        Order order = await _repo.GetOrderByIdAsync(orderId);
        VerifyAddress()
        if (ModelState.IsValid)
        {
            await _repo.CreateShippment(ship);

            return RedirectToAction(nameof(Index));
        }
        return View(ship);
    }


    #region Shipping API

    public void VerifyAddress(ShippingAddress shippingAddress)
    {
        var api = new AddressValidationApi();
        var xPBUnifiedErrorStructure = true;
        var minimalAddressValidation = true;

        Address toVerify = new()
        {
            AddressLines =
            {
                shippingAddress.AddressLine1,
                shippingAddress.AddressLine2
            },
            CityTown = shippingAddress.CityTown,
            CountryCode = shippingAddress.Country,
            PostalCode = $"{shippingAddress.PostalCode}",
            StateProvince = shippingAddress.StateAbr.ToString(),
        };

        try
        {
            // Address validation
            Address result = api.VerifyAddress(toVerify, xPBUnifiedErrorStructure, minimalAddressValidation);
            Debug.WriteLine(result);
        }
        catch (ApiException e)
        {
            Debug.Print("Exception when calling AddressValidationApi.VerifyAddress: " + e.Message);
            Debug.Print("Status Code: " + e.ErrorCode);
            Debug.Print(e.StackTrace);
        }

        #endregion
    }
}
