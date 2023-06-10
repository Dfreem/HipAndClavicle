
using static HipAndClavicle.ViewModels.ShoppingCartViewModel;

namespace HipAndClavicle.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartRepo _shoppingCartRepo;
    private readonly ICustRepo _custRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public AppUser Owner { get; set; }

    public ShoppingCartController(IServiceProvider services, IHttpContextAccessor accessor)
    {
        _shoppingCartRepo = services.GetRequiredService<IShoppingCartRepo>();
        _custRepo = services.GetRequiredService<ICustRepo>();
        _contextAccessor = accessor;
        _signInManager = services.GetRequiredService<SignInManager<AppUser>>();
        _userManager = _signInManager.UserManager;
        Owner = _userManager.GetUserAsync(User).Result ?? GetDefaultUserAsync().Result;

    }
    /// <summary>
    /// Gets or creates a default user and a n
    /// </summary>
    /// <returns></returns>
    public async Task<AppUser> GetDefaultUserAsync()
    {
        AppUser owner = await _userManager.FindByNameAsync("DEFAULT") ?? new()
        {
            UserName = "DEFAULT"
        };
        await _userManager.CreateAsync(owner, "");

        owner.Cart = await _shoppingCartRepo.GetShoppingCartByOwnerId(owner.Id) ?? new()
        {
            Owner = owner
        };
        return owner;

    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ShoppingCartViewModel viewModel = new()
        {
            ShoppingCart = Owner.Cart,
        };

        return View(viewModel);
    }

    // This action method adds a listing to the cart with the specified quantity
    // TODO: For testing Will be changed to add items from catalog.
    [HttpPost]
    public async Task<IActionResult> AddToCart(int listingId, int quantity = 1)
    {

        // Find the listing with the given listingId
        var listing = await _custRepo.GetListingByIdAsync(listingId);

        if (listing == null)
        {
            return NotFound();
        }

        // Create a new ShoppingCartItem with the shoppingCartId, listing, and quantity
        var shoppingCartItem = new ShoppingCartItem
        {

        };
        Owner.Cart.Items.Add(shoppingCartItem);
        await _shoppingCartRepo.UpdateShoppingCartAsync(Owner.Cart);

        return RedirectToAction("Index", "ShoppingCart");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart([Bind("itemId, qty")] int itemId, int qty)
    {


        var item = await _shoppingCartRepo.GetOrderItemByIdAsync(itemId);
        if (item == null)
        {
            return NotFound();
        }

        item.Qty = qty;
        await _shoppingCartRepo.UpdateItemAsync(item);
        return RedirectToAction("Index", "ShoppingCart");
    }

    // Removes single item from cart
    public async Task<IActionResult> RemoveFromCart(int itemId)
    {
        Owner.Cart.Items.Remove(await _shoppingCartRepo.GetShoppingCartItemByIdAsync(itemId));
        await _shoppingCartRepo.UpdateShoppingCartAsync(Owner.Cart);
        await _userManager.UpdateAsync(Owner);
        return RedirectToAction("Index", "ShoppingCart");
    }

    // Removes all items from cart
    [HttpPost]
    public async Task<IActionResult> ClearCart(string cartId)
    {
        var httpContext = _contextAccessor.HttpContext;
        Owner.Cart = new();
        await _userManager.UpdateAsync(Owner);
        return RedirectToAction("Index", "ShoppingCart");
    }

    //    // Helper method to get the shopping cart from the cookie
    //    private SimpleShoppingCart GetShoppingCartFromCookie()
    //    {
    //        var cartCookie = _contextAccessor.HttpContext.Request.Cookies[_shoppingCartCookieName];
    //        if (cartCookie == null)
    //        {
    //            // If the cart cookie doesn't exist, create an empty SimpleShoppingCart
    //            return new SimpleShoppingCart { Items = new List<SimpleCartItem>() };
    //        }
    //        else
    //        {
    //            // Deserialize the SimpleShoppingCart from the cart cookie
    //            return JsonConvert.DeserializeObject<SimpleShoppingCart>(cartCookie);
    //        }
    //    }

    //    // Helper method to save the shopping cart in the cookie
    //    private void SetShoppingCartToCookie(SimpleShoppingCart shoppingCart)
    //    {
    //        // Serialize the shopping cart and save it in the cookie
    //        var cartJson = JsonConvert.SerializeObject(shoppingCart);
    //        _contextAccessor.HttpContext.Response.Cookies.Append(_shoppingCartCookieName, cartJson, new CookieOptions()); // Cookie will expire once browser is closed
    //    }

    //    // Helper method to empty the cart
    //    private void ClearShoppingCartCookie()
    //    {
    //        var emptyCart = new SimpleShoppingCart { Items = new List<SimpleCartItem>() };
    //        var json = STJ.JsonSerializer.Serialize(emptyCart);
    //        _contextAccessor.HttpContext.Response.Cookies.Append(_shoppingCartCookieName, json, new CookieOptions()); // Cookie will expire once browser is closed
    //    }
}

