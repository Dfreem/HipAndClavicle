using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HipAndClavicle.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<IActionResult> Index()
        {
            /*var userId = LoginVM.UserName;
            var cart = await _shoppingCartRepository.GetOrCreateShoppingCartAsync(userId);
            return View(cart);*/

            var userId = User.Identity.Name;
            var cart = await _shoppingCartRepository.GetOrCreateShoppingCartAsync(userId);
            return View(cart);
        }
    }
}

