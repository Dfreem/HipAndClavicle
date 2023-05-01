
using Microsoft.CodeAnalysis.Rename;
using RestSharp.Serialization;

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
    private readonly string _pbMerchantId;


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
        _pbMerchantId = config["PitneyBowes:DefaultMerchentId"]!;
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
            //TODO make modal for selecting shipping rates
            Rate rate = new(carrier: Carrier.USPS, serviceId: Services.PM, parcelType: ParcelType.PKG, inductionPostalCode: toAddress.PostalCode);
            Document label = new(type: "SHIPPING_LABEL", contentType: Document.ContentTypeEnum.URL, size: Document.SizeEnum._4X6, fileFormat: Document.FileFormatEnum.PDF, printDialogOption: Document.PrintDialogOptionEnum.EMBEDPRINTDIALOG);

            List<Parameter> shippingOptions = new List<Parameter>()
            {
                new Parameter("SHIPPING_ID", _pbMerchantId),
                new Parameter("ADD_TO_MANIFEST", "false"),
                new Parameter("HIDE_TOTAL_CARRIER_CHARGE", "false"),
                new Parameter("PRINT_CUSTOM_MESSAGE_1", "Print this label"),
                new Parameter("SHIPPING_LABEL_RECEIPT", "NO_OPTIONS")
            };

            Parcel package = new(
                dimension: svm.PackageDimension,
                weight: svm.ParcelWeight,
                valueOfGoods: svm.ValueOfGoods,
                currencyCode: "USD");

            Shipment newShipment = new(

                fromAddress: shipFrom,
                toAddress: toAddress,
                parcel: package,
                rates: new() { rate },
                documents: new() { label },
                shipmentOptions: shippingOptions
            );

            Shipment? toShip = CreateLabel(newShipment);

            return RedirectToAction(nameof(ViewLabel));

        }

        _toast.Error("Could not find order in system");
        return View(svm);
    }

    private IActionResult ViewLabel(Shipment shipment)
    {
        return View(shipment.Documents);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
    public Shipment CreateLabel(Shipment shipment)
    {
        Configuration.Default.BasePath = _pbBasePath;
        Configuration.Default.OAuthApiKey = _pbApiKey;
        Configuration.Default.OAuthSecret = _pbSecret;

        var apiClient = new ShipmentApi(Configuration.Default);

        return apiClient.CreateShipmentLabel($"{DateTime.Now.Millisecond}", shipment, xPBUnifiedErrorStructure: true, xPBIntegratorCarrierId: "898644");

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
        return new ShippingAddress()
        {
            AddressLine1 = address.AddressLines[0],
            AddressLine2 = address.AddressLines[1],
            CityTown = address.CityTown,
            Country = address.CountryCode,
            PostalCode = address.PostalCode
        };
    }

    #endregion

}
