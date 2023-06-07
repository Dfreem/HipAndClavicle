using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using HipAndClavicle.ViewModels.Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HipAndClavicle.Controllers;
[Route("create-payment-intent")]
[ApiController]
public class PaymentController : Controller
{
    INotyfService _toast;
    IConfiguration _config;
    private readonly string? _stripeKey;

    public PaymentController(IServiceProvider services, IConfiguration configuration)
    {
        _toast = services.GetRequiredService<INotyfService>();
        _config = configuration;
        _stripeKey = _config["StripeKey"];
    }
    public IActionResult Index()
    {
        if (_stripeKey is null)
        {
            _toast.Warning("**Administrator** stripe api key must be set in order to activate payments.");
            return RedirectToAction("Checkout");
        }
        return View();
    }

    /// <summary>
    /// Create and register a <see cref="Stripe.Customer"></see>
    ///  with the Stripe Api https://github.com/stripe/stripe-dotnet
    /// </summary>
    /// <param name="customer">the AppUser who made the purchase</param>
    /// <returns>A <see cref="Stripe.Customer"></see></returns>
    public async Task<Customer?> CreateStripeCustomerAsync(AppUser customer)
    {
        if (customer.Address is null)
        {
            _toast.Error($"No address found for user {customer.FName}");
            await Task.CompletedTask;
            return null;
        }
        var options = new CustomerCreateOptions()
        {
            Email = customer.Email,
            Address =
            {
                Line1 = customer.Address!.AddressLine1,
                City = customer.Address!.CityTown,
                Country = "us",
                PostalCode = customer.Address!.PostalCode,
                State = customer.Address.StateAbr.ToDescriptionString()
            },

            Name = $"{customer.FName} {customer.LName}",
            Phone = customer.PhoneNumber
        };

        var service = new CustomerService();
        return await service.CreateAsync(options);
    }
    [HttpPost]
    public ActionResult Create(PaymentIntentCreateRequest request)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
        {
            Amount = CalculateOrderAmount(request.Items),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        });

        return Json(new { clientSecret = paymentIntent.ClientSecret });
    }
    /// <summary>
    /// Use the ammount on an item in order to calculate the total cost.
    /// Amount Documentation -> <inheritdoc cref="PaymentIntentCreateOptions.Amount"/>
    /// </summary>
    /// <param name="items">json cerialized Order Item</param>
    /// <returns>total amount for the payment being created.</returns>
    private long? CalculateOrderAmount(Item[] items)
    {
        
    }
}

