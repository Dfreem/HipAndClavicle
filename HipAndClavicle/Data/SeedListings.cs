using Listing = HipAndClavicle.Models.Listing;

namespace HipAndClavicle.Data
{
    public class SeedListings
    {
        public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
        {
            Color col1 = new Color()
            {
                ColorName = "Victorian Lace",
                HexValue = "#e9836f",
                RGB = (233, 131, 111)
            };
            Color col2 = new Color()
            {
                ColorName= "Carrot Orange",
                HexValue = "#e85405",
                RGB = (232, 84, 5)
            };

            Color col3 = new Color()
            {
                ColorName = "Canary Yellow",
                HexValue = "#ffd447",
                RGB = (255, 212, 71)
            };
            await context.AddRangeAsync(col1, col2 , col3 );
            await context.SaveChangesAsync();

            Product butterfly = await context.Products.Where(p => p.Name == "Butterfly Test").FirstOrDefaultAsync();
            Product dragon = await context.Products.Where(p => p.Name == "Dragon Test").FirstOrDefaultAsync();


            Listing listing1 = new Listing()
            {
                Price = 20.00d,
                Colors = 
                {
                    col1
                },
                ListingProduct = butterfly
            };
            Listing listing2 = new Listing()
            {
                Price = 20.00d,
                Colors =
                {
                    col2
                },
                ListingProduct = butterfly
            };
            Listing listing3 = new Listing()
            {
                Price = 20.00d,
                Colors =
                {
                    col3
                },
                ListingProduct = butterfly
            };
            Listing listing4 = new Listing()
            {
                Price = 20.00d,
                Colors =
                {
                    col1
                },
                ListingProduct = dragon
            };
            await context.AddRangeAsync(listing1, listing2, listing3 , listing4 );
            await context.SaveChangesAsync();

            ColorFamily cf1 = new ColorFamily()
            {
                ColorFamilyName = "Reds",
                Color = col2
            };
            ColorFamily cf2 = new ColorFamily()
            {
                ColorFamilyName = "Yellows",
                Color = col2
            };
            ColorFamily cf3 = new ColorFamily()
            {
                ColorFamilyName = "Yellows",
                Color = col3
            };
            ColorFamily cf4 = new ColorFamily()
            {
                ColorFamilyName = "Reds",
                Color = col1
            };
            await context.AddRangeAsync(cf1, cf2, cf3, cf4);
            await context.SaveChangesAsync();
        }
    }
}
