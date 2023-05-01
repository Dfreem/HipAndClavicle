using HipAndClavicle.Models;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class CustomerProductCatalogController : Controller
    {
        private readonly ICustRepo _repo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CustomerProductCatalogController(ICustRepo repo, ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _context = context;
            _userManager = userManager;
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
            var product = await _repo.GetProductByIdAsync(productId);
            crVM.Review.Reviewer =  currentUser;
            crVM.Review.ReviewedProductId = productId;
            crVM.Product = product;
            await _repo.AddReviewAsync(crVM);
            return RedirectToAction("CustFindListings", "CustomerProductCatalog");
        }
    }
}