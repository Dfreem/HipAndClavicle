using Microsoft.AspNetCore.Mvc;

namespace HipAndClavicle.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
