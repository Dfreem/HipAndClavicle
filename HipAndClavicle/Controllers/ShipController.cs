
using HipAndClavicle.Repositories;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Identity.Client;

namespace HipAndClavicle.Controllers;

public class ShipController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly IShippingRepo _repo;
    private readonly INotyfService _toast;
    private readonly string _pbBasePath;
    private readonly string _pbApiKey;
    private readonly string _pbSecret;

    public ShipController(IServiceProvider services, IConfiguration config)
    {
        _services = services;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _repo = services.GetRequiredService<IShippingRepo>();
        _toast = services.GetRequiredService<INotyfService>();

        _pbBasePath = config["PitneyBowes:BasePath"]!;
        _pbApiKey = config["PitneyBowes:Key"]!;
        _pbSecret = config["PitneyBowes:Secret"]!;
    }

    public IActionResult Shipping(Order? toShip)
    {
        toShip ??= new Order();
        ShippingVM shippingVM = new()
        {
            OrderToShip = toShip,
            Address = toShip.Address,
            Customer = toShip.Purchaser,
            Merchant = toShip.Purchaser,
            NewShipment = new()
        };
        return View(toShip);
    }

    // POST: Ship/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Shipping(int orderId)
    {
        Order order = await _repo.GetOrderByIdAsync(orderId);


        if (ModelState.IsValid)
        {
            //await _repo.CreateShippment(new());

            return RedirectToAction(nameof(Index));
        }
        return View();
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

    }

    public Document CreateLabel(ShippingVM svm)
    {
        Configuration.Default.BasePath = _pbBasePath;
        Configuration.Default.OAuthApiKey = _pbApiKey;
        Configuration.Default.OAuthSecret = _pbSecret;

        var api = new ShipmentApi(Configuration.Default);
        var xPBTransactionId = $"{DateTime.Now.Millisecond}";
        bool xPBUnifiedErrorsStructure = true;
        var xPBIntegratorCarrierId = "898644";

        try
        {

        }
    }

    public Address GetAddress(ShippingVM svm)
    {
        var shippingAddress = svm.Address;

     return new Address()
        {
            AddressLines = { shippingAddress.AddressLine1, shippingAddress.AddressLine2 },
            CityTown = shippingAddress.CityTown,
            // TODO add this to ShippingAddress model. Lower case, 2 char.
            CountryCode = "us",
            PostalCode = $"{shippingAddress.PostalCode}",
            StateProvince = shippingAddress.StateAbr.ToString(),
            Residential = shippingAddress.Residential

        };
    }
}

    //public Shipment CreateShipment()
    //{

    //}

    #endregion

}
