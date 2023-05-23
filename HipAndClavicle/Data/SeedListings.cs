using HipAndClavicle.Models.JunctionTables;
using Listing = HipAndClavicle.Models.Listing;

namespace HipAndClavicle.Repositories
{
    public class SeedListings
    {
        public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
        {
            if (await context.Listings.AnyAsync())
            {
                return;
            }
            #region MakeColors
            Color victLace = new Color()
            {
                ColorName = "Victorian Lace",
                HexValue = "#e9836f",
                //RGB = (233, 131, 111)
            };
            Color carOrg = new Color()
            {
                ColorName= "Carrot Orange",
                HexValue = "#e85405",
                //RGB = (232, 84, 5)
            };

            Color canYl = new Color()
            {
                ColorName = "Canary Yellow",
                HexValue = "#ffd447",
                //RGB = (255, 212, 71)
            };
            Color frenchVan = new Color()
            {
                ColorName = "French Vanilla",
                HexValue = "#fff5b0"
            };
            await context.NamedColors.AddRangeAsync(victLace, carOrg , canYl);
            await context.SaveChangesAsync();
            #endregion

            #region MakeProducts
            Product butterfly = new Product()
            {
                Category = ProductCategory.ButterFlys,
                Name = "ListingButterfly",
                AvailableColors = { victLace, carOrg, canYl },
                SetSizes = new()
                {
                    new SetSize() { Size = 20 }
                }
            };
            Product dragon = new Product()
            {
                Category = ProductCategory.Dragons,
                Name = "ListingDragon",
                AvailableColors = { victLace, carOrg, canYl },
                SetSizes = new()
                {
                    new SetSize() { Size = 20 }
                }
            };
            await context.Products.AddRangeAsync( butterfly, dragon );
            await context.SaveChangesAsync();
            #endregion

            #region MakeListings
            Listing listing1 = new Listing()
            {
                Price = 20.00d,
                Colors = {victLace},
                ListingProduct = butterfly,
                ListingTitle = "Butterflies in Victorian Lace",
                ListingDescription = "Really great butterflies lorem ipsum etc etc"
            };
            await context.Listings.AddAsync(listing1);
            await context.SaveChangesAsync();
            var listingColorAssoc1 = new ListingColorJT()
            {
                ListingColor = victLace,
                Listing = listing1
            };
            await context.ListingColorsJT.AddAsync(listingColorAssoc1);
            await context.SaveChangesAsync();
            //listing1.Colors.Add(victLace);
            //await context.SaveChangesAsync();

            Listing listing2 = new Listing()
            {
                Price = 20.00d,
                Colors = { carOrg },
                ListingProduct = butterfly,
                ListingTitle = "Butterflies in Carrot Orange",
                ListingDescription = "Really great butterflies lorem ipsum etc etc"
            };
            await context.Listings.AddAsync(listing2);
            await context.SaveChangesAsync();
            var listingColorAssoc2 = new ListingColorJT()
            {
                ListingColor = carOrg,
                Listing = listing2
            };
            await context.ListingColorsJT.AddAsync(listingColorAssoc2);
            await context.SaveChangesAsync();
            //listing2.Colors.Add(carOrg);
            //await context.SaveChangesAsync();

            Listing listing3 = new Listing()
            {
                Price = 20.00d,
                Colors = { canYl },
                ListingProduct = butterfly,
                ListingTitle = "Butterflies in Canary Yellow",
                ListingDescription = "Really great butterflies lorem ipsum etc etc"
            };
            await context.Listings.AddAsync(listing3);
            await context.SaveChangesAsync();
            var listingColorAssoc3 = new ListingColorJT()
            {
                ListingColor = canYl,
                Listing = listing3
            };
            await context.ListingColorsJT.AddAsync(listingColorAssoc3);
            await context.SaveChangesAsync();
            //listing3.Colors.Add(canYl);
            //await context.SaveChangesAsync();

            Listing listing4 = new Listing()
            {
                Price = 20.00d,
                Colors = { victLace },
                ListingProduct = dragon,
                ListingTitle = "Dragons in Victorian Lace",
                ListingDescription = "Really great dragons lorem ipsum etc etc"
            };
            await context.Listings.AddAsync(listing4);
            await context.SaveChangesAsync();
            var listingColorAssoc4 = new ListingColorJT()
            {
                ListingColor = victLace,
                Listing = listing4
            };
            await context.ListingColorsJT.AddAsync(listingColorAssoc4);
            await context.SaveChangesAsync();
            //listing4.Colors.Add(victLace);
            //await context.SaveChangesAsync();

            //await context.Listings.AddRangeAsync(listing1, listing2, listing3, listing4);
            await context.SaveChangesAsync();

            //Listing listing1 = new Listing()
            //{
            //    Price = 20.00d,
            //    Colors = { col1 },
            //    ListingProduct = butterfly,
            //    ListingTitle = "Butterflies in Victorian Lace",
            //    ListingDescription = "Really great butterflies lorem ipsum etc etc"
            //};
            //await context.AddAsync( listing1 );
            //await context.SaveChangesAsync();

            //Listing listing2 = new Listing()
            //{
            //    Price = 20.00d,
            //    Colors = { col1 },
            //    ListingProduct = butterfly,
            //    ListingTitle = "Butterflies in Carrot Orange",
            //    ListingDescription = "Really great butterflies lorem ipsum etc etc"
            //};
            //await context.AddAsync(listing2);
            //await context.SaveChangesAsync();

            //Listing listing3 = new Listing()
            //{
            //    Price = 20.00d,
            //    Colors = { col1 },
            //    ListingProduct = butterfly,
            //    ListingTitle = "Butterflies in Canary Yellow",
            //    ListingDescription = "Really great butterflies lorem ipsum etc etc"
            //};
            //await context.AddAsync(listing3);
            //await context.SaveChangesAsync();

            //Listing listing4 = new Listing()
            //{
            //    Price = 20.00d,
            //    Colors = { col1 },
            //    ListingProduct = dragon,
            //    ListingTitle = "Dragons in Victorian Lace",
            //    ListingDescription = "Really great dragons lorem ipsum etc etc"
            //};
            //await context.AddAsync(listing4);
            //await context.SaveChangesAsync();
            #endregion

            #region MakeColorFamilies
            ColorFamily cf1 = new ColorFamily()
            {
                ColorFamilyName = "Reds",
                Color = carOrg
            };
            ColorFamily cf2 = new ColorFamily()
            {
                ColorFamilyName = "Yellows",
                Color = carOrg
            };
            ColorFamily cf3 = new ColorFamily()
            {
                ColorFamilyName = "Yellows",
                Color = canYl
            };
            ColorFamily cf4 = new ColorFamily()
            {
                ColorFamilyName = "Reds",
                Color = victLace
            };
            await context.ColorFamilies.AddRangeAsync(cf1, cf2, cf3, cf4);
            await context.SaveChangesAsync();
            #endregion
        }
    }
}
