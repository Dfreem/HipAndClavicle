using Color = HipAndClavicle.Models.Color;
namespace HipAndClavicle.Data;

public static class SeedData
{
    static ApplicationDbContext? _context;
    static UserManager<AppUser>? _userManager;

    public static async Task Init(IServiceProvider services, ApplicationDbContext context)
    {
        _context = context;
        _userManager = services.GetRequiredService<UserManager<AppUser>>();

        if (_userManager!.Users.Any())
        {
            return;
        }
        AppUser michael = new()
        {
            UserName = "michael123",
            Email = "paulsonM@my.lanecc.edu",
            EmailConfirmed = true,
            FName = "Michael",
            LName = "Pauslon",
            Address = "123 fake st. Eugene, OR 97448"
        };

        AppUser devin = new()
        {
            UserName = "dfreem987",
            Email = "freemand@my.lanecc.edu",
            EmailConfirmed = true,
            FName = "Devin",
            LName = "Freeman",
            Address = "123 fake st. Eugene, OR 97448"
        };

        AppUser steven = new()
        {
            UserName = "steven123",
            Email = "bradyS@my.lanecc.edu",
            EmailConfirmed = true,
            FName = "Steven",
            LName = "Brady",
            Address = "123 fake st. Eugene, OR 97448"
        };

        AppUser nehemiah = new()
        {
            UserName = "nehemiah123",
            Email = "johnn@my.lanecc.edu",
            EmailConfirmed = true,
            FName = "Nehemiah",
            LName = "John",
            Address = "123 fake st. Eugene, OR 97448"
        };
        _ = await _userManager!.CreateAsync(devin, "!BassCase987");
        _ = await _userManager!.CreateAsync(nehemiah, "@Password123");
        _ = await _userManager!.CreateAsync(michael, "@Password123");
        _ = await _userManager!.CreateAsync(steven, "@Password123");

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

        Product butterfly = new()
        {
            Category = ProductCategory.ButterFlys,
            Name = "Butterfly Test",
            InStock = true,
<<<<<<< HEAD
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
        Order order1 = new()
        {
            Items = new() { item1!},
            DateOrdered = DateTime.Now,
            PurchaserId = await _userManager.GetUserIdAsync(devin),
            ShippingAddress = devin!.Address!,
            TotalPrice = 25.00d,

        };
        Order order2 = new()
        {
            Items = new() { item1!},
            DateOrdered = DateTime.Now,
            PurchaserId = michael!.Id,
            ShippingAddress = michael!.Address!,
            TotalPrice = 125.00d,
        };
        Order order3 = new()
        {
            Items = new() { item1! },
            DateOrdered = DateTime.Now,
            PurchaserId = steven!.Id,
            ShippingAddress = steven!.Address!,
            TotalPrice = 25.00d,

        };
        Order order4 = new()
        {
            Items = new() { item1! },
            DateOrdered = DateTime.Now,
            PurchaserId = nehemiah!.Id,
            ShippingAddress = nehemiah!.Address!,
            TotalPrice = 125.00d
        };
        await _context.Orders.AddRangeAsync(order1, order2, order3, order4);
        await _context.SaveChangesAsync();
    }
}


=======
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
        Order order1 = new()
        {
            Items = new() { item1!},
            DateOrdered = DateTime.Now,
            PurchaserId = await _userManager.GetUserIdAsync(devin),
            ShippingAddress = devin!.Address!,
            TotalPrice = 25.00d,

        };
        Order order2 = new()
        {
            Items = new() { item1!},
            DateOrdered = DateTime.Now,
            PurchaserId = michael!.Id,
            ShippingAddress = michael!.Address!,
            TotalPrice = 125.00d,
        };
        Order order3 = new()
        {
            Items = new() { item1! },
            DateOrdered = DateTime.Now,
            PurchaserId = steven!.Id,
            ShippingAddress = steven!.Address!,
            TotalPrice = 25.00d,

        };
        Order order4 = new()
        {
            Items = new() { item1! },
            DateOrdered = DateTime.Now,
            PurchaserId = nehemiah!.Id,
            ShippingAddress = nehemiah!.Address!,
            TotalPrice = 125.00d
        };
        await _context.Orders.AddRangeAsync(order1, order2, order3, order4);
        await _context.SaveChangesAsync();
>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
