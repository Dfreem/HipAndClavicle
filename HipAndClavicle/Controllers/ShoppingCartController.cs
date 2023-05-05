using System.Linq;
using System.Security.Claims;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
//using static HipAndClavicle.ViewModels.SimpleCartModel;

namespace HipAndClavicle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepo _shoppingCartRepo;
        private readonly ICustRepo _custRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ShoppingCartController(IShoppingCartRepo shoppingCartRepository, ICustRepo custRepo, IHttpContextAccessor httpContextAccessor)
        {
            _shoppingCartRepo = shoppingCartRepository;
            _custRepo = custRepo;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var httpContext = _contextAccessor.HttpContext;
            string cartId = GetCartId();
            ShoppingCartViewModel viewModel;

            // Determine if the user is logged in and retrieve the shopping cart accordingly
            if (User.Identity.IsAuthenticated)
            {
                string ownerId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ShoppingCart shoppingCart = await _shoppingCartRepo.GetOrCreateShoppingCartAsync(cartId, ownerId);

                viewModel = new ShoppingCartViewModel
                {
                    CartId = shoppingCart.CartId,
                    ShoppingCartItems = await _shoppingCartRepo.GetShoppingCartItemsAsync(shoppingCart.ShoppingCartItems),
                };
            }
            else
            {
                SimpleShoppingCart simpleShoppingCart = GetShoppingCartFromCookie();

                viewModel = new ShoppingCartViewModel
                {
                    CartId = cartId,
                    ShoppingCartItems = simpleShoppingCart.Items.Select(item => new ShoppingCartItemViewModel(item)).ToList(),
                };
            }

            return View(viewModel);
        }

        // Helper method to get the cart ID for the current user
        private string GetCartId()
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                return null;
            }
        }

        // This action method adds a listing to the cart with the specified quantity
        // TODO: For testing Will be changed later.
        [HttpPost]
        public async Task<IActionResult> AddToCart(int listingId, int quantity = 1)
        {
            // Get the cart ID
            var cartId = GetCartId();

            if (cartId != null)
            {
                // Handle logged-in users

                var httpContext = _contextAccessor.HttpContext;
                string ownerId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Get the shopping cart using the cart ID
                var shoppingCart = await _shoppingCartRepo.GetOrCreateShoppingCartAsync(cartId, ownerId);

                // Find the listing with the given listingId
                var listing = await _custRepo.GetListingByIdAsync(listingId);

                if (listing == null)
                {
                    return NotFound();
                }

                // Create a new ShoppingCartItem with the shoppingCartId, listing, and quantity
                var shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = shoppingCart.Id,
                    ListingItem = listing,
                    Quantity = quantity
                };

                await _shoppingCartRepo.AddShoppingCartItemAsync(shoppingCartItem);
            }
            else
            {
                // Handle non-logged-in users

                var shoppingCart = GetShoppingCartFromCookie();
                var listing = await _custRepo.GetListingByIdAsync(listingId);

                // Check if the listing already exists in the shopping cart
                var simpleCartItem = shoppingCart.Items.FirstOrDefault(item => item.ListingId == listingId);
                if (simpleCartItem != null)
                {
                    simpleCartItem.Qty += quantity;
                }
                else
                {
                    simpleCartItem = new SimpleCartItem
                    {
                        ListingId = listing.ListingId,
                        Name = listing.ListingTitle,
                        Desc = listing.ListingDescription,
                        Qty = quantity,
                        ItemPrice = listing.Price
                    };
                    shoppingCart.Items.Add(simpleCartItem);
                }
                // Save the updated shopping cart in the cookie
                SetShoppingCartToCookie(shoppingCart);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int itemId, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Handle logged-in users
                var item = await _shoppingCartRepo.GetCartItem(itemId);
                if (item == null)
                {
                    return NotFound();
                }

                item.Quantity = quantity;
                await _shoppingCartRepo.UpdateItemAsync(item);
            }
            else
            {
                // Handle non-logged-in users
                var simpleShoppingCart = GetShoppingCartFromCookie();
                var simpleCartItem = simpleShoppingCart.Items.FirstOrDefault(item => item.Id == itemId);
                if (simpleCartItem != null)
                {
                    simpleCartItem.Qty = quantity;
                }
                else
                {
                    return NotFound();
                }
                SetShoppingCartToCookie(simpleShoppingCart);
            }
            
            return RedirectToAction("Index");
        }

        // Helper method to get the shopping cart from the cookie
        private SimpleShoppingCart GetShoppingCartFromCookie()
        {
            var cartCookie = _contextAccessor.HttpContext.Request.Cookies["Cart"];
            if (cartCookie == null)
            {
                // If the cart cookie doesn't exist, create an empty SimpleShoppingCart
                return new SimpleShoppingCart { Items = new List<SimpleCartItem>() };
            }
            else
            {
                // Deserialize the SimpleShoppingCart from the cart cookie
                return JsonConvert.DeserializeObject<SimpleShoppingCart>(cartCookie);
            }
        }

        // Helper method to save the shopping cart in the cookie
        private void SetShoppingCartToCookie(SimpleShoppingCart shoppingCart)
        {
            // Serialize the shopping cart and save it in the cookie
            var cartJson = JsonConvert.SerializeObject(shoppingCart);
            _contextAccessor.HttpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions()); // Cookie will expire once browser is closed
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Handle logged-in users
                var item = await _shoppingCartRepo.GetCartItem(itemId);
                if (item == null)
                {
                    return NotFound();
                }

                await _shoppingCartRepo.RemoveItemAsync(item);
            }
            else
            {
                // Handle non-logged-in users
                var simpleShoppingCart = GetShoppingCartFromCookie();
                var simpleCartItem = simpleShoppingCart.Items.FirstOrDefault(item => item.Id == itemId);
                if (simpleCartItem != null)
                {
                    simpleShoppingCart.Items.Remove(simpleCartItem);
                }
                else
                {
                    return NotFound();
                }
                SetShoppingCartToCookie(simpleShoppingCart);
            }
            return RedirectToAction("Index");
        }
    }

}

