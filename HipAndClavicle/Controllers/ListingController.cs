using HipAndClavicle.Models.JunctionTables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Controllers
{
    [Authorize (Roles = "Admin")]
    public class ListingController : Controller
    {
        private readonly ICustRepo _repo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ListingController(ICustRepo repo, ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _context = context;
            _userManager = userManager;
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
            if(addListingVM.ImageFile != null)
            {
                addListingVM.SingleImage = await ExtractImageAsync(addListingVM.ImageFile);
            }
            var colorsToAdd = new List<Color>();
            foreach (var colorId in addListingVM.SelectedColors)
            {
                var colorToAdd = await _repo.GetColorByIdAsync(colorId);
                colorsToAdd.Add(colorToAdd);
            }

            var productToAssoc = await _repo.GetProductByIdAsync(addListingVM.ListingProductId);

            addListingVM.NewListing = new Listing()
            {
                ListingTitle = addListingVM.Title,
                ListingDescription = addListingVM.Description,
                Colors = colorsToAdd,
                ListingProduct = productToAssoc,
                SingleImage = addListingVM.SingleImage

            };
            await _context.Listings.AddAsync(addListingVM.NewListing);

            foreach (var col in colorsToAdd)
            {
                var newListingColorAssoc = new ListingColorJT()
                {
                    Listing = addListingVM.NewListing,
                    ListingColor = col
                };
                await _context.ListingColorsJT.AddAsync(newListingColorAssoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("CustFindListings", "Customer");
        }

        public async Task<Image> ExtractImageAsync(IFormFile imageFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                return new Image()
                {
                    ImageData = memoryStream.ToArray(),
                    Width = 200
                };
            }
        }


    }
}
