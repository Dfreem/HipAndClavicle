using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class ListingController : Controller
    {

        public IActionResult Index()
        {
            return View();
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
    }
}
