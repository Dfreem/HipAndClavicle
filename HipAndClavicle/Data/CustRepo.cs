namespace HipAndClavicle.Data
{
    public class CustRepo : ICustRepo
    {
        private readonly IServiceProvider _services;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CustRepo(IServiceProvider services, ApplicationDbContext context)
        {
            _services = services;
            _context = context;
            _userManager = services.GetRequiredService<UserManager<AppUser>>();
        }
        public async Task<List<Listing>> GetAllListings()
        {
            var listings = await _context.Listings.ToListAsync();
            return listings;
        }
        public async Task<List<Listing>> GetItemsByColorFamily(string colorFamily)
        {
            //var searchColors = await _context.ColorFamilies
            //    .Include()
            //var listings = await _context.Listings
            //    .Include(l => l.Colors)

            //Just returns all listings for now

            var listings = await _context.Listings.ToListAsync();
            return listings;
        }
    }
}
