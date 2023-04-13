using Listing = HipAndClavicle.Models.Listing;

namespace HipAndClavicle.Data
{
    public class SeedItems
    {
        public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
        {
            Color col1 = new Color()
            {
                ColorName = "Midnight Plumb Metalic",
                HexValue = "#2d2063",
                RGB = (45, 32, 99)
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
        }
    }
}
