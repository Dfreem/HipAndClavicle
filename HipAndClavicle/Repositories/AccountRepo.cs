namespace HipAndClavicle.Repositories;

public class AccountRepo : IAccountRepo
{
    private readonly IServiceProvider _services;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AccountRepo(IServiceProvider services, ApplicationDbContext context)
    {
        _services = services;
        _context = context;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();
    }

    public async Task<ShippingAddress?> FindUserAddress(AppUser user)
    {
        return await _context.Addresses.FindAsync(user.ShippingAddressId);
    }

    public async Task UpdateUserAddressAsync(AppUser user)
    {
        await _userManager.UpdateAsync(user);
    }
}