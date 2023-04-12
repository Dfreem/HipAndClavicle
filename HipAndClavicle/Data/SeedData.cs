using Color = HipAndClavicle.Models.Color;
namespace HipAndClavicle.Data;

public static class SeedData
{
    

    public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
    {
        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        if (await context.NamedColors.AnyAsync())
        { return; }

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
        await context.NamedColors.AddRangeAsync(blue, green, red);

        Product butterfly = new()
        {
            Category = ProductCategory.ButterFlys,
            Name = "Butterfly Test",
            InStock = true,
            QuantityOnHand = 100,
            Colors = { red, blue, green },
            SetSizes = new()
            {
                new SetSize() { Size = 20 }
            }
        };
        // TODO add a check to see if the app user has purchased the product before being able to leave a review.


        Product dragon = new()
        {
            Category = ProductCategory.Dragons,
            Name = "Dragon Test",
            InStock = true,
            QuantityOnHand = 100,
            Colors = { red, blue, green },
            SetSizes = new()
            {
                new SetSize() { Size = 20 }
            }
        };
        Product dragonfly = new()
        {
            Category = ProductCategory.Dragonflys,
            Name = "Butterfly",
            InStock = true,
            QuantityOnHand = 100,
            Colors = { red, blue, green },
            SetSizes = new()
            {
                new SetSize() { Size = 20 }
            }
        };
        //await context.Products.AddRangeAsync(dragon, dragonfly, butterfly);

        await context.Products.AddRangeAsync(dragon, dragonfly, butterfly);

        var devin = await userManager.FindByNameAsync("dfreem987");
        var michael = await userManager.FindByNameAsync("michael123");
        var steven = await userManager.FindByNameAsync("steven123");
        var nehemiah = await userManager.FindByNameAsync("nehemiah123");

        Order order1 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = devin!,
            ShippingAddress = devin!.Address!,
            TotalPrice = 25.00d,

        };
        Order order2 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = michael!,
            ShippingAddress = michael!.Address!,
            TotalPrice = 125.00d,
        };
        Order order3 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = steven!,
            ShippingAddress = steven!.Address!,
            TotalPrice = 25.00d,

        };
        Order order4 = new()
        {
            DateOrdered = DateTime.Now,
            Purchaser = nehemiah!,
            ShippingAddress = nehemiah!.Address!,
            TotalPrice = 125.00d
        };

        SetSize two = new() { Size = 2 };
        SetSize seven = new() { Size = 7 };
        SetSize ten = new() { Size = 10 };
        SetSize fifteen = new() { Size = 15 };
        await context.SetSizes.AddRangeAsync(two, seven, ten, fifteen);

        OrderItem item1 = new()
        {
            Item = dragonfly,
            ItemType = ProductCategory.Dragonflys,
            ItemColor = { blue },
            SetSize = new SetSize() { Size = 6 }
        };
        OrderItem item2 = new()
        {
            Item = butterfly,
            ItemColor = { red },
            ItemType = ProductCategory.ButterFlys,
            SetSize = new() { Size = 15 }
        };
        OrderItem item3 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            SetSize = new() { Size = 22 }
        };
        
        OrderItem item4 = new()
        {
            Item = dragon,
            ItemType = ProductCategory.Dragons,
            SetSize = new() { Size = 22 }
        };

        await context.OrderItems.AddRangeAsync(item1, item2, item3);

        order1.Items.Add(item1);
        order2.Items.Add(item2);
        order3.Items.Add(item3);

        await context.Orders.AddRangeAsync(order1, order2, order3, order4);
        await context.SaveChangesAsync();
    }
}

