using HipAndClavicle.Models.JunctionTables;
using Microsoft.CodeAnalysis.CSharp;

namespace HipAndClavicle.Repositories
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
                .Include(l => l.Colors)
                .Include(l => l.ListingProduct)
                .ThenInclude(p => p.Colors)
                .Include(l => l.ListingProduct)
                .ThenInclude(p => p.ProductImage)
                .Include(l => l.ListingColorJTs)
                .ThenInclude(lc  => lc.ListingColor)
                .ToListAsync();

            return listings;
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .ToListAsync();
            return products;
        }

        public async Task<List<Color>> GetAllColorsAsync()
        {
            var colors = await _context.NamedColors
                .ToListAsync();
            return colors;
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
            await _context.ColorFamilies.AddAsync(colorFamily);
            await _context.SaveChangesAsync();
        }

        public async Task AddListingAsync(Listing listing)
        {
            await _context.Listings.AddAsync(listing);
            await _context.SaveChangesAsync();
        }

        public async Task AddListingImageAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddColorToListing(Listing listing, Color color)
        {
            var listingColorAssociation = new ListingColorJT()
            {
                Listing = listing,
                ListingColor = color
            };
            await _context.ListingColorsJT.AddAsync(listingColorAssociation);
        }
        #endregion
    }
}
