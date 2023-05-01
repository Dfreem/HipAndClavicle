using System.Threading.Tasks;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICustRepo _custRepo;
        private readonly UserManager<AppUser> _userManager;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, ICustRepo custRepo)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _custRepo = custRepo;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Retrieve the shopping cart for the logged in user
                var user = await _userManager.GetUserAsync(User);
                var cart = await _shoppingCartRepository.GetShoppingCartByUser(user.Id);
                var cartItems = await _shoppingCartRepository.GetItemsAsync(user.Id);

                // Create a new ShoppingCartViewModel
                ShoppingCartViewModel shoppingCartVM = new ShoppingCartViewModel();
                // Set CartTotal property
                shoppingCartVM.CartTotal = cart.CartTotal;


                /*// Retrieve the additional data from the Listing model for each shopping cart item
                foreach (var item in shoppingCart.ShoppingCartItems)
                {
                    var listing = await _custRepo.GetListingByIdAsync(item.Product.ListingId);
                    item.Product.ListingTitle = listing.ListingTitle;
                    item.Product.ListingDescription = listing.ListingDescription;
                    item.Product.ListingImage = listing.Images.FirstOrDefault();
                }*/

                // Build the view model
                var viewModel = new ShoppingCartViewModel();
                //viewModel.IsUserLoggedIn = user != null;

                if (cart != null)
                {
                    viewModel.CartTotal = cart.CartTotal;

                    foreach (var item in cartItems)
                    {
                        var listing = await _custRepo.GetListingByIdAsync(item.Product.ProductId);
                        var product = listing.ListingProduct;

                        var itemViewModel = new ShoppingCartItemViewModel
                        {
                            Name = listing.ListingTitle,
                            Desc = listing.ListingDescription,
                            Img = listing.Images[0],
                            ItemPrice = listing.Price,
                            Qty = item.Quantity,
                        };

                        viewModel.ShoppingCartItems.Add(itemViewModel);
                    }
                }


                return View(viewModel);
            }
            else
            {
                return View();
            }


            /*var shoppingCart = await _shoppingCartRepository.GetOrCreateShoppingCartAsync(GetShoppingCartId());
            //await AddToCart(1); // for testing adds another item when page is reloaded
            var viewModel = new ShoppingCartViewModel
            {
                Items = shoppingCart.ShoppingCartItems.Select(item => new ShoppingCartItemViewModel
                {
                    ShoppingCartItemId = item.,
                    ListingId = item.Listing.ListingId,
                    Title = item.Listing.ListingTitle,
                    Description = item.Listing.ListingDescription,
                    Price = item.Listing.Price,
                    Quantity = item.Quantity,
                    Images = item.Listing.Images,
                }).ToList()
            };

            return View(viewModel);
        }*/

            /*private string GetShoppingCartId()
            {
                //TODO: use session to store shopping cart id for user?
                string shoppingCartId = HttpContext.Session?.GetString("ShoppingCartId");

                if (shoppingCartId == null)
                {
                    shoppingCartId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("ShoppingCartId", shoppingCartId);
                }
                return shoppingCartId;

                //return "testShoppingCartId"; // for testing
                //return "testShoppingCartId2"; // for testing
                //return "cart1";
            }*/

            /*// testing accessing listing in DB by adding to cart
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
            }*/
        }
    }
}

