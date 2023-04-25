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

        [HttpPost]
        public async Task<IActionResult> AddListing(AddListingVM addListingVM)
        {
            using (var memoryStream = new MemoryStream())
            {
                await addListingVM.ListingImageFile.CopyToAsync(memoryStream);
                Image initialImage = new Image()
                {
                    ImageData = memoryStream.ToArray(),
                    Width = 200
                };
                addListingVM.ListingImages.Add(initialImage);
                await _repo.AddListingImageAsync(initialImage);
                await _repo.AddListingAsync((Listing)addListingVM);
                return RedirectToAction("Listing");

            }
        }
    }
}
