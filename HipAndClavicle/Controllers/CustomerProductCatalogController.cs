using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class CustomerProductCatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchByColor() 
        {
            return View();
        }
    }
}
