using HipAndClavicle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotyfService _toast;

        public AccountController(IServiceProvider services, ApplicationDbContext context)
        {
            _toast = services.GetRequiredService<INotyfService>();
            _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
            _userManager = _signInManager.UserManager;
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM lvm)
        {

            var result = await _signInManager.PasswordSignInAsync(lvm.UserName, lvm.Password, isPersistent: lvm.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == lvm.UserName);

                if (user.IsDeleted)
                {
                    _toast.Error("Invalid username/password.\n");

                    ModelState.AddModelError("", "Invalid username/password.");
                    return View(lvm);
                }

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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                _toast.Error(ModelState.ValidationState.ToDescriptionString());
                return View(model);
            }
            if (model.Password != model.ConfirmPassword)
            {
                _toast.Error("passwords did not match");
                return View(model);
            }
            AppUser newUser = new()
            {
                FName = model.FName,
                LName = model.LName,
                UserName = model.UserName,
                Email = model.Email ?? ""
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: newUser.IsPersistent);
                _toast.Success("Successfully Registered new user " + newUser.UserName);
                //return RedirectToAction("Index", "Home");
                return RedirectToAction("Create", "ShippingAddresses");
            }
            else
            {
                _toast.Error("There was an Error\n" + result.Errors.ToString());
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            _toast.Success("You are now signed out, Goodbye!");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CustomerShippingAddress()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            AppUser currentUser = _userManager.Users.Include(x => x.Address)
                .FirstOrDefault(x => x.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                return BadRequest();
            }
            var appUserVm = new AppUserVm
            {
                Id = currentUser.Id,
                FName = currentUser.FName,
                LName = currentUser.LName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                IsPersistent = true,
                ShippingAddressId = currentUser.ShippingAddressId,
                Address = currentUser.Address
            };

            return View(appUserVm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(AppUserVm model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userManager.Users.FirstOrDefault(x => x.Id == model.Id);
                    if (user != null)
                    {
                        user.FName = model.FName;
                        user.LName = model.LName;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Email = model.Email;

                        IdentityResult result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            _toast.Success("Profile Successfully Updated");
                            return RedirectToAction(nameof(Index), "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error updating user Profile.");
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            AppUser? currentUser = _userManager.Users.Include(x => x.Address)
               .FirstOrDefault(x => x.UserName == User.Identity.Name);

            if (currentUser != null)
            {
                currentUser.IsDeleted = true;
                await _userManager.UpdateAsync(currentUser);
            }
            await _signInManager.SignOutAsync();
            _toast.Success("You are now signed out, Goodbye!");
            return RedirectToAction("Index", "Home");

        }
    }
}
