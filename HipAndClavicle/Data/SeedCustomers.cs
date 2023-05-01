

using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HipAndClavicle.Data
{
    public static class SeedCustomers
    {
        //TODO: Use this for seed customers if needed
        public static async Task Seed(IServiceProvider services, ApplicationDbContext context)
        {
            const string ANNE_PASS = "@Password123";
            UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            if (userManager.Users.Where(u => u.UserName == "Anne123") == null)
            {
                return;
            }
            ShippingAddress addy1 = new ShippingAddress()
            {
                AddressLine1 = "321 New St",
                CityTown = "Anytown",
                StateAbr = State.OK,
                PostalCode = "12345",
                Country = "USA"
            };
            #region anneDetails
            AppUser anne = new AppUser()
            {
                UserName = "Anne123",
                Email = "anne123@IHopeThisIsntARealEmailDomain.nope",
                EmailConfirmed = true,
                FName = "Anne",
                LName = "Smith",
                Address = addy1,
                PhoneNumber = "1234567890",
            };

            AppUser ann = new AppUser()
            {
                UserName = "Ann89",
                Email = "ann89@notarealemail.com",
                EmailConfirmed = true,
                FName = "Ann",
                LName = "Brown",
                Address = addy1,
                PhoneNumber = "9876543210",
            };

            AppUser anneMarie = new AppUser()
            {
                UserName = "AnneMarie77",
                Email = "annemarie77@fakemail.net",
                EmailConfirmed = true,
                FName = "Anne-Marie",
                LName = "Jones",
                Address = addy1,
                PhoneNumber = "5555555555",
            };

            AppUser ane = new AppUser()
            {
                UserName = "AneDoe",
                Email = "anedoe@somemail.com",
                EmailConfirmed = true,
                FName = "Ane",
                LName = "Doe",
                Address = addy1,
                PhoneNumber = "1111111111",
            };

            AppUser annie = new AppUser()
            {
                UserName = "AnnieSmith",
                Email = "anniesmith@anothermail.net",
                EmailConfirmed = true,
                FName = "Annie",
                LName = "Smith",
                Address = addy1,
                PhoneNumber = "2222222222",
            };

            AppUser an = new AppUser()
            {
                UserName = "An21",
                Email = "an21@madeupmail.com",
                EmailConfirmed = true,
                FName = "An",
                LName = "Lee",
                Address = addy1,
                PhoneNumber = "3333333333",
            };
            #endregion annDetails

            #region addTheAnnes
            _ = await userManager!.CreateAsync(anne, ANNE_PASS);
            _ = await userManager!.CreateAsync(ann, ANNE_PASS);
            _ = await userManager!.CreateAsync(anneMarie, ANNE_PASS);
            _ = await userManager!.CreateAsync(ane, ANNE_PASS);
            _ = await userManager!.CreateAsync(annie, ANNE_PASS);
            _ = await userManager!.CreateAsync(an, ANNE_PASS);
            #endregion addTheAnnes

            #region addCustRole
            if (await roleManager.FindByNameAsync("Customer") is null)
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            //IdentityRole customerRole = await roleManager.FindByNameAsync("Customer");
            if (!await userManager.IsInRoleAsync(anne, "Customer"))
            {
                await userManager.AddToRoleAsync(anne, "Customer");
                await userManager.AddToRoleAsync(ann, "Customer");
                await userManager.AddToRoleAsync(anneMarie, "Customer");
                await userManager.AddToRoleAsync(ane, "Customer");
                await userManager.AddToRoleAsync(annie, "Customer");
                await userManager.AddToRoleAsync(an, "Customer");
            }
            #endregion addCustRole

            #region AddOrders
            SetSize eleven = new SetSize() { Size = 10 };
            var butterfly = await context.Products.Where(p => p.Name == "ListingButterfly").FirstOrDefaultAsync();
            var dragon = await context.Products.Where(p => p.Name == "ListingDragon").FirstOrDefaultAsync();
            var newColor = new Color()
            {
                ColorName = "newYellow",
                HexValue = "#edcd2b"
            };
            await context.NamedColors.AddAsync(newColor);

            Order order1 = new Order()
            {
                DateOrdered = DateTime.Now,
                Purchaser = anne!,
                Address = anne!.Address!,
                TotalPrice = 30.00d
            };
            Order order2 = new Order()
            {
                DateOrdered = DateTime.Now,
                Purchaser = ane!,
                Address = ane!.Address!,
                TotalPrice = 30.00d
            };

            OrderItem item1 = new OrderItem()
            {
                Item = butterfly,
                ItemType = ProductCategory.ButterFlys,

            };
            await context.OrderItems.AddAsync(item1);
            order1.Items.Add(item1);

            await context.Orders.AddRangeAsync(order1, order2);
            #endregion AddOrders

            if (await context.Reviews.AnyAsync())
            {
                return;
            }
            Review rev1 = new Review()
            {
                Reviewer = anne,
                Message = "These were great butterflys.",
                VerifiedOrderId = order1.OrderId,
                ReviewedProductId = butterfly!.ProductId
            };
            Review rev2 = new Review()
            {
                Reviewer = ane,
                Message = "These were super great butterflys.",
                VerifiedOrderId = order1.OrderId,
                ReviewedProductId = butterfly!.ProductId
            };
            await context.Reviews.AddRangeAsync(rev1, rev2);
            butterfly.Reviews.Add(rev1);
            butterfly.Reviews.Add(rev2);

            await context.SaveChangesAsync();
        }
    }
}
