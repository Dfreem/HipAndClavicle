using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HipAndClavicle
{
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
            _stripeKey = _config["StripeKey"]??"";
        }
        public IActionResult Index()
        {
            if (_stripeKey == "")
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
    }
}

