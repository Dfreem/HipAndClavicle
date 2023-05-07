using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Repositories
{
    public static class SeedUsers
    {
        public static async Task Seed(IServiceProvider services)
        {
            UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
            if (userManager!.Users.Any())
            {
                return;

            }
            ShippingAddress fakeSt = new()
            {
                AddressLine1 = "123 fake st." ,

                CityTown = "Eugene",
                StateAbr = State.OR,
                PostalCode = "97448",
                Country = "USA"
            };
            AppUser michael = new()
            {
                UserName = "michael123",
                Email = "paulsonM@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Michael",
                LName = "Pauslon",
                Address = fakeSt,
                PhoneNumber = "555-555-5555"

            };

            AppUser devin = new()
            {
                UserName = "dfreem987",
                Email = "freemand@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Devin",
                LName = "Freeman",
                Address = fakeSt,
                PhoneNumber = "555-555-5555"
            };

            AppUser steven = new()
            {
                UserName = "steven123",
                Email = "bradyS@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Steven",
                LName = "Brady",
                PhoneNumber = "555-555-5555",
                Address = fakeSt
            };

            AppUser nehemiah = new()
            {
                UserName = "nehemiah123",
                Email = "johnn@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Nehemiah",
                LName = "John",
                PhoneNumber = "555-555-5555",
                Address = fakeSt
            };

            AppUser testAdmin = new()
            {
                UserName = "TestAdmin123",
                Email = "test@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Test",
                LName = "Admin",
                PhoneNumber = "555-555-5555",
                Address = fakeSt
            };
            _ = await userManager!.CreateAsync(devin, "!BassCase987");
            _ = await userManager!.CreateAsync(nehemiah, "@Password123");
            _ = await userManager!.CreateAsync(michael, "@Password123");
            _ = await userManager!.CreateAsync(steven, "@Password123");
            _ = await userManager!.CreateAsync(testAdmin, "@Password123");
        }
    }
}

