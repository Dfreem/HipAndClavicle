


using ShipEngineSDK;
using ShipEngineSDK.CreateLabelFromRate;
using ShipEngineSDK.CreateLabelFromShipmentDetails;
using ShipEngineSDK.GetRatesWithShipmentDetails;

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
        return View(shippingVM);
    }
    /// <summary>
    /// The http-post verison of Ship takes everything entered on the ship screen and creates a shipping label that can be printed or downloaded.
    /// </summary>
    /// <param name="svm">The <see cref="ShippingVM"/> View Model containing form data</param>
    /// <returns>Navigate to Label view / Print</returns>
    [HttpPost]
    public async Task<IActionResult> Ship([Bind("Merchant, Customer, OrderToShip, NewPackage, SelectedService, ShipDate")] ShippingVM svm)
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
        ShipEngineSDK.CreateLabelFromShipmentDetails.Result result = await CreatLabelAsync(svm);

        return View("ViewLabel", result);

    }

    public async Task<ShipEngineSDK.CreateLabelFromShipmentDetails.Result> CreatLabelAsync(ShippingVM svm)
    {
        ShipEngineSDK.CreateLabelFromShipmentDetails.Params rateParams = new()
        {
            Shipment = new()
            {
                CarrierId = svm.SelectedService!.CarrierId,
                Packages = new()
                {
                    svm.NewPackage
                },
                ShipFrom = new()
                {
                    AddressLine1 = svm.Merchant.Address.AddressLine1,
                    AddressLine2 = svm.Merchant.Address.AddressLine2,
                    CityLocality = svm.Merchant.Address.CityTown,
                    Name = svm.Merchant.FName + " " + svm.Merchant.LName,
                    PostalCode = svm.Merchant.Address.PostalCode,
                    CountryCode = Country.US,
                    StateProvince = svm.Merchant.Address.StateAbr.ToString(),
                    Phone = svm.Merchant.PhoneNumber,
                },
                ShipTo = new()
                {
                    AddressLine1 = svm.Customer.Address.AddressLine1,
                    AddressLine2 = svm.Customer.Address.AddressLine2,
                    CityLocality = svm.Customer.Address.CityTown,
                    Name = svm.Customer.FName + " " + svm.Customer.LName,
                    PostalCode = svm.Customer.Address.PostalCode,
                    CountryCode = Country.US,
                    StateProvince = svm.Customer.Address.StateAbr.ToString(),
                    Phone = svm.Customer.PhoneNumber,
                },
                ServiceCode = svm.SelectedService.ServiceCode,
            },
            LabelFormat = svm.LabelFormat,
            LabelLayout = svm.LabelLayout
        };

        try
        {
            return await _shipEngine.CreateLabelFromShipmentDetails(rateParams);
        }
        catch (ShipEngineException ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
