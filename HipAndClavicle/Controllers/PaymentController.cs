using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HipAndClavicle
{
    public class PaymentController : Controller
    {
        INotyfService _toast;
        IConfiguration _config;
        private readonly string _stripeKey;

        public PaymentController(IServiceProvider services, IConfiguration configuration)
        {
            _toast = services.GetRequiredService<INotyfService>();
            _config = configuration;
            _config[""]

        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create and register a <see cref="Stripe.Customer"/></see>
        ///  with the [Stripe api](https://github.com/stripe/stripe-dotnet)
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        async Task<Customer?> CreateStripeCustomerAsync(AppUser customer)
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

