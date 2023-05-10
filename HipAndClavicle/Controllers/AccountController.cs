namespace HipAndClavicle.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotyfService _toast;
    private readonly IShippingRepo _shippingRepo;
    private readonly IAccountRepo _accountRepo;

    public AccountController(IServiceProvider services, ApplicationDbContext context)
    {
        _toast = services.GetRequiredService<INotyfService>();
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _userManager = _signInManager.UserManager;
        _shippingRepo = services.GetRequiredService<IShippingRepo>();
        _accountRepo = services.GetRequiredService<IAccountRepo>();
    }

    public async Task<IActionResult> Index()
    {
        string userName = _signInManager.Context.User.Identity!.Name!;
        var user = await _userManager.FindByNameAsync(userName);
        user!.Address = await _accountRepo.FindUserAddress(user);
        UserProfileVM uvm = new()
        {
            CurrentUser = user!,
            
        };
        return View(uvm);
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
            return RedirectToAction("Index", "Home");
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
    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserProfileVM upvm)
    {
        if (upvm.NewPassword != null && 
            upvm.NewPassword == upvm.ConfirmPassword &&
            upvm.CurrentPassword is not null)
        {
           await _userManager.ChangePasswordAsync(upvm.CurrentUser, upvm.CurrentPassword, upvm.NewPassword);
        }
        if (upvm.NewPassword != upvm.ConfirmPassword)
        {
            _toast.Error("Passwords do not match, pleas re-enter new password");
            return RedirectToAction("Index", upvm);
        }
        await _accountRepo.UpdateUserAddressAsync(upvm.CurrentUser, upvm.CurrentUser.Address);
        
        _toast.Success("Your information was updated");
        return RedirectToAction("Index", upvm);
    }
}
