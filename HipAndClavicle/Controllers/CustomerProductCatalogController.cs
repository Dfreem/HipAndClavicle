using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class CustomerProductCatalogController : Controller
    {
        private readonly ICustRepo _repo;
        private readonly ApplicationDbContext _context;
        public CustomerProductCatalogController(ICustRepo repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
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
            return View(listing);
        }
    }
}
