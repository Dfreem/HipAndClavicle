
using HipAndClavicle.Repositories;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;

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

    public async Task<IActionResult> Shipping(int orderId)
    {
        var toShip = await _repo.GetOrderByIdAsync(orderId);
        ShippingVM shippingVM = new()
        {
            OrderToShip = toShip,
            Address = toShip.Address,
            Customer = toShip.Purchaser,
            Merchant = toShip.Purchaser,
            NewShipment = new()
        };
        return View(shippingVM);
    }

    // POST: Ship/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Shipping(ShippingVM svm)
    {

        if (ModelState.IsValid)
        {
            CreateLabel(svm);

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
    public void CreateLabel(ShippingVM svm)
    {

        Configuration.Default.BasePath = _pbBasePath;
        Configuration.Default.OAuthApiKey = _pbApiKey;
        Configuration.Default.OAuthSecret = _pbSecret;

        var apiClient = new ShipmentApi(Configuration.Default);

        // Create the shipment request
        var shipment = new Shipment
        {
            Parcel = new Parcel
            {
                Weight = svm.ParcelWeight
            },
            FromAddress = ConvertAddress(svm.Customer.Address!),
            ToAddress = (Address)svm,
            ShipmentType = Shipment.ShipmentTypeEnum.OUTBOUND,
        };

        try
        {
            // Create the shipment
            Shipment result = apiClient.CreateShipmentLabel($"{DateTime.Now.Millisecond}", shipment);

            Debug.WriteLine(result);

        }
        catch (ApiException e)
        {
            Debug.Print("Exception when calling ShipmentApi.CreateShipment: " + e.Message);
            Debug.Print("Status Code: " + e.ErrorCode);
            Debug.Print(e.StackTrace);
        }
    }

    public Address ConvertAddress(ShippingAddress shippingAddress)
    {
       
        return new Address()
        {
            AddressLines = { shippingAddress!.AddressLine1, shippingAddress?.AddressLine2 },
            CityTown = shippingAddress!.CityTown,
            // TODO add this to ShippingAddress model. Lower case, 2 char.
            CountryCode = "us",
            PostalCode = $"{shippingAddress.PostalCode}",
            StateProvince = shippingAddress.StateAbr.ToString(),
            Residential = shippingAddress.Residential

        };

    }
        #endregion

}
