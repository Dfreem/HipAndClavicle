using HipAndClavicle.Models.JunctionTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Controllers
{
    public class ListingController : Controller
    {
        private readonly ICustRepo _repo;
        private readonly ApplicationDbContext _context;
        public ListingController(ICustRepo repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
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
                ListingProduct = productToAssoc

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

            //using (var memoryStream = new MemoryStream())
            //{

            //    //addListingVM.NewListing = new Listing()
            //    //{
            //    //    Colors = addListingVM.ListingColors
            //    //};
            //    //await addListingVM.ListingImageFile.CopyToAsync(memoryStream);
            //    //Image newImage = new()
            //    //{
            //    //    ImageData = memoryStream.ToArray(),
            //    //    Width = 100
            //    //};
            //    //addListingVM.NewListing.Images = new();
            //    //addListingVM.NewListing.ListingProduct = await _context.Products.FirstAsync(p => p.Category == addListingVM.NewListing.ListingProduct.Category);
            //    //addListingVM.NewListing.Images.Add(newImage);
            //    //await _context.Images.AddAsync(newImage);
            //    //_context.Products.Update(addListingVM.NewListing.ListingProduct);
            //    await _context.Listings.AddAsync(addListingVM.NewListing);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("CustListings");
            //}
            //public async Task<IActionResult> AddListing(AddListingVM addListingVM)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        var listing = (Listing)addListingVM;
            //        //await addListingVM.ListingImageFile.CopyToAsync(memoryStream);
            //        //Image initialImage = new Image()
            //        //{
            //        //    ImageData = memoryStream.ToArray(),
            //        //    Width = 200
            //        //};
            //        //addListingVM.NewListing.Images.Add(initialImage);
            //        //await _repo.AddListingImageAsync(initialImage);
            //        await _repo.AddListingAsync(listing);
            //        return RedirectToAction("Listing");

            //    }
            //}
        }


    }
}
