


using ShipEngineSDK;
using ShipEngineSDK.CreateLabelFromRate;
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
    private readonly ShipEngine _shipEngine;

    public ShipController(IServiceProvider services, IConfiguration config)
    {
        _services = services;
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _userManager = _signInManager.UserManager;
        _repo = services.GetRequiredService<IShippingRepo>();
        _toast = services.GetRequiredService<INotyfService>();

        // Ship Engine Key for Shipments
        _shipEngineKey = config["ShipEngine"]!;
        _shipEngine = new ShipEngine(_shipEngineKey);
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
            Carriers = await _shipEngine.ListCarriers(),
        };
        shippingVM.ShippingRates = await _shipEngine.GetRatesWithShipmentDetails(GetRateParams(shippingVM.Merchant, shippingVM.Customer, shippingVM.NewPackage));
        return View(shippingVM);
    }
    /// <summary>
    /// The http-post verison of Ship takes everything entered on the ship screen and creates a shipping label that can be printed or downloaded.
    /// </summary>
    /// <param name="svm">The <see cref="ShippingVM"/> View Model containing form data</param>
    /// <returns>Navigate to Label view / Print</returns>
    [HttpPost]
    public async Task<IActionResult> Ship([Bind("Merchant, Customer, OrderToShip, NewPackage, Carrier")] ShippingVM svm)
    {
        // retrieve merchant entity from Identity stores
        var merchant = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity!.Name!);

        // retrieve merchants's address from DB
        merchant!.Address = await _repo.FindUserAddress(merchant);

        // retrieve order being shipped from DB
        var order = await _repo.GetOrderByIdAsync(svm.OrderToShip.OrderId);

        if (svm.Customer.Address is null)
        {
            _toast.Error("Address cannot be empty. Please check both to and from address'");
            return View(svm);
        }
        // ViewModels Prep 
        svm.Merchant = merchant;
        svm.OrderToShip = order;

        // setup shipping rates parameters
        Params p = GetRateParams(merchant, svm.Customer, svm.NewPackage);

        // Get Shipping Rates
        svm.ShippingRates = await _shipEngine.GetRatesWithShipmentDetails(p);
        return View(svm);

    }

    public async Task<IActionResult> ViewLabel(ShippingVM svm)
    {
        // setup shipping label parameters.
        // If needed, label format and size can be changed here,
        // as well as the download options for the label
        var @params = new ShipEngineSDK.CreateLabelFromRate.Params()
        {
            RateId = svm.ShippingRates!.RateResponse.Rates[svm.SelectedCarrier].RateId,
            ValidateAddress = ValidateAddress.ValidateAndClean,
            LabelFormat = LabelFormat.PDF,
            LabelLayout = LabelLayout.FourBySix,
            LabelDownloadType = ShipEngineSDK.CreateLabelFromRate.LabelDownloadType.Url,
        };
        try
        {
            // create a label and navigates to the print label screen
            ShipEngineSDK.CreateLabelFromRate.Result result = await _shipEngine.CreateLabelFromRate(@params);
            return View(result);
        }
        catch (ShipEngineException e)
        {
            Console.WriteLine("Error creating label");
            throw e;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Shipping API

    public async Task<Result> GetRatesAsync(Params p)
    {
        try
        {
            return await _shipEngine.GetRatesWithShipmentDetails(p);
        }
        catch (ShipEngineException e)
        {
            Console.WriteLine("Error creating label");
            throw e;
        }
    }
    public Params GetRateParams(AppUser merchant, AppUser customer, ShipEngineSDK.CreateLabelFromRate.Package newPackage)
    {
        ShippingAddress shipTo = customer.Address!;
        ShippingAddress shipFrom = merchant.Address!;

        var rateParams = new Params()
        {
            Shipment = new Shipment()
            {
                ServiceCode = "usps_priority_mail",
                ShipFrom = new Address()
                {
                    Name = merchant.FName + " " + merchant.LName,
                    AddressLine1 = shipFrom.AddressLine1,
                    CityLocality = shipFrom.CityTown,
                    StateProvince = shipFrom.StateAbr.ToString(),
                    PostalCode = shipFrom.PostalCode,
                    CountryCode = Country.US,
                    Phone = merchant.PhoneNumber
                },
                ShipTo = new Address()
                {
                    Name = customer.FName + " " + merchant.LName,
                    AddressLine1 = shipTo.AddressLine1,
                    CityLocality = shipTo.CityTown,
                    StateProvince = shipTo.StateAbr.ToString(),
                    PostalCode = shipTo.PostalCode,
                    CountryCode = Country.US,
                    Phone = merchant.PhoneNumber
                },

                Packages = new()
                {
                    new ShipEngineSDK.Common.ShipmentPackage()
                    {
                        Weight = new Weight()
                        {
                            Value = newPackage.Weight!.Value,
                            Unit = newPackage.Weight.Unit ?? WeightUnit.Ounce

                        },

                        Dimensions = new Dimensions()
                        {
                            Length = newPackage.Dimensions!.Length,
                            Width = newPackage.Dimensions.Width,
                            Height = newPackage.Dimensions.Height,
                            Unit = newPackage.Dimensions.Unit
                        }
                    }
                }
            },
            RateOptions = new RateOptions()
            {
                CarrierIds = new List<string>() { "se-4697495" },
                ServiceCodes = new List<string>() { "usps_priority_mail" }
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
