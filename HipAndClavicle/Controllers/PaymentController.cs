using Microsoft.AspNetCore.Identity;

namespace HipAndClavicle.Controllers;

[Route("/[controller]")]
public class PaymentController : Controller
{
    readonly INotyfService _toast;
    IConfiguration _config;
    UserManager<AppUser> _userManager;
    SignInManager<AppUser> _signInManager;
    private readonly string? _stripeKey;
    IAccountRepo _accountRepo;

    public PaymentController(IServiceProvider services, IConfiguration configuration)
    {
        _toast = services.GetRequiredService<INotyfService>();
        _config = configuration;
        StripeConfiguration.ApiKey = _config["StripeKey"];
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _accountRepo = services.GetRequiredService<IAccountRepo>();
    }
    [Route("create-payment-intent")]
    public async Task<IActionResult> Index(Order order)
    {
        var purchaser = await _userManager.GetUserAsync(User);
        Customer? customer = await CreateStripeCustomerAsync(purchaser);
        var paymentIntent = CreatePaymentIntent(order);
        ViewData["ClientSecret"] = paymentIntent.ClientSecret;
        return View();
    }


    ///// <summary>
    ///// Create and register a <see cref="Stripe.Customer"></see>
    /////  with the Stripe Api. See the API @ https://github.com/stripe/stripe-dotnet
    ///// </summary>
    ///// <param name="customer">the AppUser who made the purchase</param>
    ///// <returns>A <see cref="Stripe.Customer"></see></returns>
    public async Task<Customer?> CreateStripeCustomerAsync(AppUser customer)
    {
        var shippingAddress = await _accountRepo.FindUserAddress(customer!);
        
        if (customer.Address is null)
        {
            _toast.Error($"No address found for user {customer.FName}");
            await Task.CompletedTask;
            return null;
        }
             
        CustomerCreateOptions options = new()
        {
            Email = customer!.Email,
            Address = new()
            {
                Line1 = shippingAddress!.AddressLine1,
                City = shippingAddress.CityTown,
                Country = shippingAddress.Country,
                PostalCode = shippingAddress.PostalCode,
                State = shippingAddress.StateAbr.ToDescriptionString()
            },
            Name = $"{customer.FName} {customer.LName}"
        };
        
        RequestOptions requestOptions = new()
        {
            IdempotencyKey = customer!.Id
        };
        
        CustomerService service = new();
        
        return await service.CreateAsync(options, requestOptions);
    }


    /// <summary>
    /// Create a payment intent for stripe checkout.
    /// </summary>
    /// <param name="request">A <see cref="PaymentIntentCreateRequest"/></param>
    /// <returns></returns>
    [HttpPost]
    public PaymentIntent CreatePaymentIntent(Order order)
    {

        var paymentIntentService = new PaymentIntentService();
        return paymentIntentService.Create(new PaymentIntentCreateOptions
        {
            Amount = CalculateOrderAmount(order.Items),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        });

    }
    /// <summary>
    /// Use the ammount on an item in order to calculate the total cost.
    /// Amount Documentation -> <inheritdoc cref="PaymentIntentCreateOptions.Amount"/>
    /// </summary>
    /// <param name="items">json cerialized Order Item</param>
    /// <returns>total amount for the payment being created.</returns>
    private static long? CalculateOrderAmount(List<OrderItem> items)
    {
        long orderTotal = 0;
        foreach (OrderItem item in items)
        {
            orderTotal += (long)(item.AmountOrdered * item.PricePerUnit);
        }
        return orderTotal;
    }
}

