

namespace HipAndClavicle.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly INotyfService _toast;

        public AccountController(IServiceProvider services, ApplicationDbContext context)
        {
            _toast = services.GetRequiredService<INotyfService>();
            _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            LoginVM lvm = new() { ReturnUrl = returnUrl };
            return View(lvm);
        }

        //TODO
        //Add after Identity is added
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM lvm)
        {

            var result = await _signInManager.PasswordSignInAsync(lvm.UserName, lvm.Password, isPersistent: lvm.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _toast.Success("Successfully Logged in as " + lvm.UserName);
                if (!string.IsNullOrEmpty(lvm.ReturnUrl) && Url.IsLocalUrl(lvm.ReturnUrl))
                { return Redirect(lvm.ReturnUrl); }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            _toast.Error("Unable to Sign in\n" + result.ToString());

            ModelState.AddModelError("", "Invalid username/password.");
            return View(lvm);
        }
    }
}
