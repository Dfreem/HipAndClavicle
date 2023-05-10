


using ShipEngineSDK;
using ShipEngineSDK.GetRatesWithShipmentDetails;
using Params = ShipEngineSDK.GetRatesWithShipmentDetails.Params;
using Result = ShipEngineSDK.GetRatesWithShipmentDetails.Result;
using Shipment = ShipEngineSDK.GetRatesWithShipmentDetails.Shipment;

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
        var shipEngine = new ShipEngine("api_key");
        if (svm.Customer.Address is null || svm.Merchant.Address is null)
        {
            _toast.Error("Address cannot be empty. Please check both to and from address'");
            return View(svm);
        }
        Params @params = GetShippingParams(svm);
        Result rates = await GetRatesAsync(shipEngine, @params);
        if (rates.RateResponse.Status == RateStatus.Error)
        {
            _toast.Error("error creating label. Please try again later.\n" + rates.RateResponse.ToString());
            return View(svm);
        }
        return RedirectToAction("ViewLabel", rates);
    }

    private IActionResult ViewLabel(ShipEngineSDK.GetRatesWithShipmentDetails.Result result)
    {
        return View(result);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Shipping API

    public async Task<Result> GetRatesAsync(ShipEngine shipEngine, Params p)
    {
        try
        {
            Result result = await shipEngine.GetRatesWithShipmentDetails(p);
            return result;
        }
        catch (ShipEngineException e)
        {
            Console.WriteLine("Error creating label");
            throw e;
        }
    }
    public Params GetShippingParams(ShippingVM svm)
    {
        ShippingAddress shipTo = svm.Customer.Address!;
        ShippingAddress shipFrom = svm.Merchant.Address!;


        var rateParams = new Params()
        {
            Shipment = new Shipment()
            {
                ServiceCode = "usps_priority_mail",
                ShipFrom = new Address()
                {
                    Name = svm.Merchant.FName + " " + svm.Merchant.LName,
                    AddressLine1 = shipFrom.AddressLine1,
                    CityLocality = shipFrom.CityTown,
                    StateProvince = shipFrom.StateAbr.ToString(),
                    PostalCode = shipFrom.PostalCode,
                    CountryCode = Country.US,
                    Phone = svm.Merchant.PhoneNumber
                },
                ShipTo = new Address()
                {
                    Name = svm.Customer.FName + " " + svm.Merchant.LName,
                    AddressLine1 = shipTo.AddressLine1,
                    CityLocality = shipTo.CityTown,
                    StateProvince = shipTo.StateAbr.ToString(),
                    PostalCode = shipTo.PostalCode,
                    CountryCode = Country.US,
                    Phone = svm.Merchant.PhoneNumber
                },

                Packages = new()
                {
                    new ShipEngineSDK.Common.ShipmentPackage()
                    {
                        Weight = new Weight()
                        {
                            Value = svm.NewPackage.Weight.Value,
                            Unit = svm.NewPackage.Weight.Unit

                        },

                        Dimensions = new Dimensions()
                        {
                            Length = 36,
                            Width = 12,
                            Height = 24,
                            Unit = DimensionUnit.Inch
                        }
                    }
                }
            }
        };
        return rateParams;

    }



    #endregion

    #region Utility

    public async Task<IActionResult> ChangeMerchantAddress(ShippingVM svm)
    {
        await _userManager.UpdateAsync(svm.Merchant);
        return RedirectToAction("Ship");
    }

    #endregion

}
