using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class CustomerProductCatalogController : Controller
    {
        private readonly IHipRepo _repo;
        private readonly ApplicationDbContext _context;
        public CustomerProductCatalogController(IHipRepo repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchByColor() 
        {
            return View();
        }

        public async Task<IActionResult> ViewAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            var listings = await _context.Listings.ToListAsync();
            ProductListingVM productsVM = new ProductListingVM();
            productsVM.Products = products;
            productsVM.Listings = listings;
            


            return View(productsVM);
        }
    }
}