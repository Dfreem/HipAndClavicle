
namespace HipAndClavicle.Controllers;
[Authorize(Roles = "Admin")]
public class ShipController : Controller
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IShippingRepo _repo;
    private readonly INotyfService _toast;
    private readonly string _pbBasePath;
    private readonly string _pbApiKey;
    private readonly string _pbSecret;

    public ShipController(IServiceProvider services, IConfiguration config)
    {
        _services = services;
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _userManager = _signInManager.UserManager;
        _repo = services.GetRequiredService<IShippingRepo>();
        _toast = services.GetRequiredService<INotyfService>();

        _pbBasePath = config["PitneyBowes:BasePath"]!;
        _pbApiKey = config["PitneyBowes:Key"]!;
        _pbSecret = config["PitneyBowes:Secret"]!;
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
            Address shipFrom = ConvertAddress(merchant.Address);
            // TODO when an order is made, there must be a check for Oroder.Purchaser.Address
            Address toAddress = ConvertAddress(svm.OrderToShip.Purchaser.Address!);
            Parcel package = new(
                dimension: svm.PackageDimension,
                weight: svm.ParcelWeight,
                valueOfGoods: svm.ValueOfGoods,
                currencyCode: "USD");

            Shipment newShipment = new(

                fromAddress: shipFrom,
                toAddress: toAddress,
                parcel: package,
                rates: new()

            );
            CreateLabel(newShipment);

            return RedirectToAction(nameof(Index));
        }
        return View(svm);
    }


    #region Shipping API

    /// <summary>
    /// Use to verify an address before creating a label
    /// </summary>
    /// <param name="shippingAddress">The <see cref="ShippingAddress"/> representation of the address to ship to</param>
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
            // Address Verification
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
    public void CreateLabel(Shipment shipment)
    {
        Configuration.Default.BasePath = _pbBasePath;
        Configuration.Default.OAuthApiKey = _pbApiKey;
        Configuration.Default.OAuthSecret = _pbSecret;

        var apiClient = new ShipmentApi(Configuration.Default);

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

    #endregion

    #region Utility

    public Address ConvertAddress(ShippingAddress shippingAddress)
    {
        return new Address()
        {
            AddressLines = new() { shippingAddress!.AddressLine1 },
            CityTown = shippingAddress!.CityTown,
            CountryCode = shippingAddress.Country,
            PostalCode = $"{shippingAddress.PostalCode}",
            StateProvince = shippingAddress.StateAbr.ToString(),
            Residential = shippingAddress.Residential

        };

    }

    public ShippingAddress ConvertAddress(Address address)
    {
#pragma warning disable CA1305 // Specify IFormatProvider
        return new ShippingAddress()
        {
            AddressLine1 = address.AddressLines[0],
            AddressLine2 = address.AddressLines[1],
            CityTown = address.CityTown,
            Country = address.CountryCode,
            PostalCode = address.PostalCode
        };
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    #endregion

}
