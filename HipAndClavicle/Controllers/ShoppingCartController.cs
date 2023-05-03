using System.Drawing.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            if (User.Identity.IsAuthenticated)
            {
                string ownerId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                shoppingCart = await _shoppingCartRepo.GetOrCreateShoppingCartAsync(cartId, ownerId);
            }
            else
            {
                shoppingCart = GetShoppingCartFromCookie();
            }

            var viewModel = new ShoppingCartViewModel
            {
                CartId = shoppingCart.CartId,
                ShoppingCartItems = shoppingCart.ShoppingCartItems.Select(item => new ShoppingCartItemViewModel(item)).ToList(),
            };

            return View(viewModel);
        }

        private async Task<ShoppingCart> GetShoppingCartForNonLoggedInUserAsync()
        {
            var httpContext = _contextAccessor.HttpContext;
            var cartCookie = httpContext.Request.Cookies["CartId"];
            if (cartCookie != null)
            {
                var shoppingCart = new ShoppingCart { CartId = cartCookie };
                var shoppingCartItems = await _shoppingCartRepo.GetShoppingCartItemsAsync(cartCookie);
                shoppingCart.ShoppingCartItems = shoppingCartItems;
                return shoppingCart;
            }
            else
            {
                var newCartId = Guid.NewGuid().ToString();
                httpContext.Response.Cookies.Append("CartId", newCartId, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
                return new ShoppingCart { CartId = newCartId };
            }
        }

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

                // Create a new ShoppingCartItem with the listing and quantity
                var shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = shoppingCart.Id,
                    ListingItem = listing,
                    Quantity = quantity
                };

                //await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
                await _shoppingCartRepo.AddShoppingCartItemAsync(shoppingCartItem);
            }
            else
            {
                // Handle non-logged-in users

                var shoppingCart = GetShoppingCartFromCookie();
                var listing = await _custRepo.GetListingByIdAsync(listingId);

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

                SetShoppingCartToCookie(shoppingCart);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        private ShoppingCart GetShoppingCartFromCookie()
        {
            var cartCookie = _contextAccessor.HttpContext.Request.Cookies["Cart"];
            if (cartCookie == null)
            {
                return new ShoppingCart { ShoppingCartItems = new List<ShoppingCartItem>() };
            }
            else
            {
                return JsonConvert.DeserializeObject<ShoppingCart>(cartCookie);
            }
        }

        private void SetShoppingCartToCookie(ShoppingCart shoppingCart)
        {
            var cartJson = JsonConvert.SerializeObject(shoppingCart);
            _contextAccessor.HttpContext.Response.Cookies.Append("Cart", cartJson, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
        }
    }



}

