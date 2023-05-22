using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = HipAndClavicle.Models.Color;
namespace HipAndClavicle.Repositories;

public static class SeedData
{


    public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
    {
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        if (await context.NamedColors.AnyAsync())
        { return; }

        #region Colors
        Color blue = new()
        {
            ColorName = "blue",
            HexValue = "#0000ff",
            RGB = (0, 0, 255)
        };
        Color red = new()
        {
            ColorName = "red",
            HexValue = "#ff0000",
            RGB = (255, 0, 0)
        };
        Color green = new()
        {
            ColorName = "green",
            HexValue = "#00ff00",
            RGB = (0, 255, 0)
        };
        Color newYellow = new()
        {
            ColorName = "newYellow",
            HexValue = "#edcd2b"
        };
        Color newRed = new()
        {
            ColorName = "newRed",
            HexValue = "#ef3939"
        };
        #endregion

        #region Sizes
        SetSize two = new() { Size = 2 };
        SetSize seven = new() { Size = 7 };
        SetSize ten = new() { Size = 10 };
        SetSize twelve = new() { Size = 12 };
        SetSize fifteen = new() { Size = 15 };
        SetSize twenty = new() { Size = 20 };
        #endregion

        // TODO add a check to see if the app user has purchased the product before being able to leave a review.

        Product butterfly = new()
        {
            Category = ProductCategory.ButterFlys,
            Name = "Butterfly Test",
            AvailableColors = { red, blue, green },
            SetSizes = new()
            {
                two,
                seven,
                twelve
            },
        };


        Product dragon = new()
        {
            Category = ProductCategory.Dragons,
            Name = "Dragon Test",
            AvailableColors = { red, blue, green },
            SetSizes = new()
            {
                two,
                ten,
                fifteen
            }
        };
        Product dragonfly = new()
        {
            Category = ProductCategory.Dragonflys,
            Name = "Butterfly",
            AvailableColors = { red, blue, green },
            SetSizes = new()
            {
                seven,
                twelve
            }
        };
        //await context.Products.AddRangeAsync(dragon, dragonfly, butterfly);

        await context.Products.AddRangeAsync(dragon, dragonfly, butterfly);

        var devin = await userManager.FindByNameAsync("dfreem987");
        var michael = await userManager.FindByNameAsync("michael123");
        var steven = await userManager.FindByNameAsync("steven123");
        var nehemiah = await userManager.FindByNameAsync("nehemiah123");
        var anne = await userManager.FindByNameAsync("Anne123");
        var ane = await userManager.FindByNameAsync("AneDoe");

        Order order1 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = devin!,
            Address = devin!.Address!           

        };
        Order order2 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = michael!,
            Address = michael!.Address!
        };
        Order order3 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = steven!,
            Address = steven!.Address!

        };
        Order order4 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = nehemiah!,
            Address = nehemiah!.Address!
        };
        Order order5 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = anne!,
            Address = anne!.Address!,
        };
        Order order6 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = ane!,
            Address = ane!.Address!,
        };

        OrderItem item1 = new()
        {
            Item = butterfly,
            ItemType = ProductCategory.ButterFlys,
            //ItemColors = { newColor, newColor2 },
            AmountOrdered = 3,
            PricePerUnit = 23.00d,

        };
        OrderItem item2 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            //ItemColors = { newColor},
            AmountOrdered = 2,
            PricePerUnit = 22.00d
        };
        OrderItem item3 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            //ItemColors = { newColor },
            AmountOrdered = 5,
            PricePerUnit = 22.00d
        };
        OrderItem item4 = new()
        {
            Item = dragonfly,
            ItemType = ProductCategory.Dragonflys,
            ItemColors = { blue },
            PricePerUnit = 2.54d,
            AmountOrdered = 5,
            SetSize = two
        };
        OrderItem item5 = new()
        {
            Item = butterfly,
            ItemColors = { red },
            ItemType = ProductCategory.ButterFlys,
            PricePerUnit = 10d,
            SetSize = fifteen
        };
        OrderItem item6 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            SetSize = ten,
            ItemColors = { red, blue},
            PricePerUnit = 12d
        };
        OrderItem item7 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            AmountOrdered = 3,
            PricePerUnit = 19.99d,
            SetSize = new() { Size = 22 },
            ItemColors = { blue },
        };

        Review rev1 = new Review()
        {
            Reviewer = anne,
            Message = "These were great butterflys.",
            VerifiedOrder = order1,
            ReviewedProduct = butterfly!
        };
        butterfly.Reviews.Add(rev1);

        Review rev2 = new Review()
        {
            Reviewer = ane,
            Message = "These were super great butterflys.",
            VerifiedOrder = order1,
            ReviewedProduct = butterfly!
        };
        butterfly.Reviews.Add(rev2);
        // I don't think i need this one

        //await context.OrderItems.AddRangeAsync(item1, item2, item3);

        order1.Items.Add(item1);
        order2.Items.Add(item2);
        order3.Items.Add(item3);
        order3.Items.Add(item4);
        order4.Items.Add(item5);
        order5.Items.Add(item6);
        order6.Items.Add(item7);

        await context.NamedColors.AddRangeAsync(blue, green, red, newYellow, newRed);
        await context.SetSizes.AddRangeAsync(two, seven, ten, twelve, fifteen, twenty);
        await context.Orders.AddRangeAsync(order1, order2, order3, order4);
        await context.Reviews.AddRangeAsync(rev1, rev2);
        await context.SaveChangesAsync();
    }
}

