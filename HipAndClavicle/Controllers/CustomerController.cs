using HipAndClavicle.Models;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustRepo _repo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;
        public CustomerController(ICustRepo repo, ApplicationDbContext context, IServiceProvider services)
        {
            _repo = repo;
            _context = context;
            _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
            _userManager = _signInManager.UserManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchByColor(string colorFamilyName)
        {
            return View();
        }

        public async Task<IActionResult> CustFindListings(string colorFamilyName)
        {
            List<Listing> listings;
            if (colorFamilyName != null)
            {
                listings = await _repo.GetListingsByColorFamilyAsync(colorFamilyName);
            }
            else
            {
                listings = await _repo.GetAllListingsAsync();
            }
            return View(listings);
        }


        public async Task<IActionResult> CustListing(int listingId)
        {
            var listing = await _repo.GetListingByIdAsync(listingId);
            var product = await _repo.GetProductByIdAsync(listing.ListingProduct.ProductId);
            var isPurchaced = false;
            AppUser currentUser = null;
            if (User.Identity.IsAuthenticated)
            {
                currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                isPurchaced = await _repo.UserPurchasedProduct(product.ProductId, currentUser);
            }
            var clVM = new CustListingVM()
            {
                User = currentUser,
                Product = product,
                Listing = listing,
                ProductIsPurchaced = isPurchaced,
            };

            return View(clVM);
        }

        public async Task<IActionResult> AddListing()
        {
            var products = await _repo.GetAllProductsAsync();
            AddListingVM theVM = new()
            {
                Products = products
            };

            return View(theVM);
        }

        public async Task<IActionResult> AddReview(int productId, int listingId)
        {
            var product = await _repo.GetProductByIdAsync(productId);


            CustReviewVM crVM = new CustReviewVM()
            {
                Product = product,
                ListingId = listingId
            };
            return View(crVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(CustReviewVM crVM, int productId)
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            crVM.Review.Reviewer = currentUser;
            crVM.Review.ReviewedProduct = await _repo.GetProductByIdAsync(productId);
            await _repo.AddReviewAsync(crVM);
            return RedirectToAction("CustFindListings", "CustomerProductCatalog");
        }
        // TODO send non-registered users an email invoice in lue of being able to review their current orders.
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var currentUser = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity!.Name!);

            var orders = await _repo.GetOrdersByCustomerId(currentUser.Id);

            return View(orders);
        }

        //[HttpPost]
        public async Task<IActionResult> Checkout(string cartId)
        {
            var cart = await _repo.GetOrCreateShoppingCartAsync(cartId);
            CheckoutVM checkout = new CheckoutVM()
            {
                Cart = cart,
            };
            return View(checkout);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM checkoutVm)
        {
            var userName = _signInManager.Context.User.Identity!.Name!;
            var currentUser = await _userManager.FindByNameAsync(userName);

            var cart = await _repo.GetCartByCustId(currentUser.Id);

            var items = new List<OrderItem>() { };
            foreach (var item in cart.ShoppingCartItems)
            {
                OrderItem itemToAdd = new OrderItem()
                {
                    ItemColors = item.ListingItem.Colors,
                    Item = item.ListingItem.ListingProduct,
                    ItemType = item.ListingItem.ListingProduct.Category,
                    AmountOrdered = item.Quantity,
                    PricePerUnit = item.ListingItem.Price,

                };
                items.Add(itemToAdd);
            }

            ShippingAddress address = new ShippingAddress()
            {
                AddressLine1 = checkoutVm.Street,
                CityTown = checkoutVm.City,
                StateAbr = checkoutVm.State,
                PostalCode = checkoutVm.Zip,
                Name = checkoutVm.Name
            };


            checkoutVm.Cart = cart;
            checkoutVm.Order = new Order()
            {
                Status = OrderStatus.Paid,
                DateOrdered = DateTime.Now,
                Address = address,
                Purchaser = currentUser!,
                Items = items

            };


            await _repo.AddOrderByCheckoutVmAsync(checkoutVm);
            await _repo.ClearShoppingCartAsync(currentUser!.Id);


            return RedirectToAction("Orders");
        }
    }
}