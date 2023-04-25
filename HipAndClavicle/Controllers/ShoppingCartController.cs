﻿using System.Threading.Tasks;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICustRepo _custRepo;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, ICustRepo custRepo)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _custRepo = custRepo;
        }

        public async Task<IActionResult> Index()
        {
            var shoppingCart = await _shoppingCartRepository.GetOrCreateShoppingCartAsync(GetShoppingCartId());
            //await AddToCart(1); // for testing
            var viewModel = new ShoppingCartViewModel
            {
                Items = shoppingCart.ShoppingCartItems.Select(item => new ShoppingCartItemViewModel
                {
                    ShoppingCartItemId = item.Id,
                    ListingId = item.Listing.ListingId,
                    Title = item.Listing.ListingTitle,
                    Description = item.Listing.ListingDescription,
                    Price = item.Listing.Price,
                    Quantity = item.Quantity,
                    Images = item.Listing.Images,
                }).ToList()
            };

            return View(viewModel);
        }

        private string GetShoppingCartId()
        {
            /*string shoppingCartId = HttpContext.Session?.GetString("ShoppingCartId");

            if (shoppingCartId == null)
            {
                shoppingCartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("ShoppingCartId", shoppingCartId);
            }
            return shoppingCartId;*/

            //return "testShoppingCartId"; // for testing
            return "testShoppingCartId2";
        }

        // testing accessing listing in DB by adding to cart
        public async Task<IActionResult> AddToCart(int id)
        {
            var listing = await _custRepo.GetListingByIdAsync(id);

            if (listing == null)
            {
              return NotFound();
            }

            var shoppingCart = await _shoppingCartRepository.GetOrCreateShoppingCartAsync(GetShoppingCartId());
            var shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartId = shoppingCart.ShoppingCartId,
                ListingId = listing.ListingId,
                Quantity = 2
            };

            await _shoppingCartRepository.AddItemAsync(shoppingCartItem);

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}

