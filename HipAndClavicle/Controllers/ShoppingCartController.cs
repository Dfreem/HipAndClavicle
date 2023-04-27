using System.Threading.Tasks;
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
            //await AddToCart(1); // for testing adds another item when page is reloaded
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
            //TODO: use session to store shopping cart id for user?
            /*string shoppingCartId = HttpContext.Session?.GetString("ShoppingCartId");

            if (shoppingCartId == null)
            {
                shoppingCartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("ShoppingCartId", shoppingCartId);
            }
            return shoppingCartId;*/

            //return "testShoppingCartId"; // for testing
            //return "testShoppingCartId2"; // for testing
            return "cart1";
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int itemId, int quantity)
        {
            var item = await _shoppingCartRepository.GetCartItemAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            item.Quantity = quantity;
            await _shoppingCartRepository.UpdateItemAsync(item);

            return RedirectToAction("Index");
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
                Quantity = 4
            };

            await _shoppingCartRepository.AddItemAsync(shoppingCartItem);

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}

