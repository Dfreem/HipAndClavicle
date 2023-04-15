using Microsoft.CodeAnalysis.CSharp;

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
        #region GetAll
        public async Task<List<Listing>> GetAllListingsAsync()
        {
            var listings = await _context.Listings
                .Include(c => c.Colors)
                .Include(p => p.ListingProduct)
                .ToListAsync();
            return listings;
        }
        #endregion

        #region GetSpecific
        public async Task<Listing> GetListingByIdAsync(int listingId)
        {
            var listing = await _context.Listings.Where(l => l.ListingId == listingId).FirstOrDefaultAsync();
            return listing;
        }
        public async Task<List<Color>> GetColorsByColorFamilyNameAsync(string colorFamilyName)
        {
            var colors = await _context.NamedColors
                .Include(c => c.ColorFamilies)
                .Where(c => c.ColorFamilies.Any(cf => cf.ColorFamilyName == colorFamilyName))
                .ToListAsync();

            return colors;
        }

        public async Task<List<Listing>> GetListingsByColorFamilyAsync(string colorFamilyName)
        {
            var colors = await GetColorsByColorFamilyNameAsync(colorFamilyName);   
            var listings = await _context.Listings
                .Include(l => l.Colors)
                .Include(l => l.ListingProduct)
                .Where(l => l.Colors.Any(c =>  colors.Contains(c))).ToListAsync();
            return listings;
        }


        #endregion

        #region MakeUpdates
        public async Task AddColorFamilyAsync(ColorFamily colorFamily)
        {
            await _context.AddAsync(colorFamily);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
