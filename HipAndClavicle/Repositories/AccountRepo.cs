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

    public async Task UpdateUserAddressAsync(AppUser user, ShippingAddress newAddress)
    {
        if (user.Address != null && _context.Addresses.Contains(user.Address))
        {
            _context.Addresses.Remove(user.Address);
        }
        newAddress.Residents.Add(user);
        user.Address = newAddress;
        await _userManager.UpdateAsync(user);
        await _context.Addresses.AddAsync(newAddress);
        await _context.SaveChangesAsync();
    }
}