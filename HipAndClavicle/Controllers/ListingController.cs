using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class ListingController : Controller
    {
        private readonly ICustRepo _repo;
        public ListingController(ICustRepo repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddListing()
        {
            var products = await _repo.GetAllProductsAsync();
            var colors = await _repo.GetAllColorsAsync();
            AddListingVM theVM = new()
            {
                Products = products,
                AvailableColors = colors
            };

            return View(theVM);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddListing()
        //{

        //}
    }
}
