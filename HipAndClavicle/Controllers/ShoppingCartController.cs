using System.Security.Claims;
using Newtonsoft.Json;

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
            ShoppingCart shoppingCart;

            // Determine if the user is logged in and retrieve the shopping cart accordingly
            if (User.Identity.IsAuthenticated)
            {
                string ownerId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                shoppingCart = await _shoppingCartRepo.GetOrCreateShoppingCartAsync(cartId, ownerId);
            }
            else
            {
                shoppingCart = GetShoppingCartFromCookie();
            }

            // Map the ShoppingCartItems to ShoppingCartItemViewModels and create the ShoppingCartViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartId = shoppingCart.CartId,
                ShoppingCartItems = await _shoppingCartRepo.GetShoppingCartItemsAsync(shoppingCart.ShoppingCartItems),
            };

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
                var shoppingCartItem = shoppingCart.ShoppingCartItems.FirstOrDefault(item => item.ListingItem.ListingId == listingId);
                if (shoppingCartItem != null)
                {
                    shoppingCartItem.Quantity += quantity;
                }
                else
                {
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ListingItem = listing,
                        Quantity = quantity
                    };
                    shoppingCart.ShoppingCartItems.Add(shoppingCartItem);
                }
                // Save the updated shopping cart in the cookie
                SetShoppingCartToCookie(shoppingCart);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        // Helper method to get the shopping cart from the cookie
        private ShoppingCart GetShoppingCartFromCookie()
        {
            var cartCookie = _contextAccessor.HttpContext.Request.Cookies["Cart"];
            if (cartCookie == null)
            {
                // If the cart cookie doesn't exist, create an empty shopping cart
                return new ShoppingCart { ShoppingCartItems = new List<ShoppingCartItem>() };
            }
            else
            {
                // Deserialize the shopping cart from the cart cookie
                return JsonConvert.DeserializeObject<ShoppingCart>(cartCookie);
            }
        }

        // Helper method to save the shopping cart in the cookie
        private void SetShoppingCartToCookie(ShoppingCart shoppingCart)
        {
            // Serialize the shopping cart and save it in the cookie
            var cartJson = JsonConvert.SerializeObject(shoppingCart);
            _contextAccessor.HttpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
        }
    }



}

