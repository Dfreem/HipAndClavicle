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
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            if (userManager!.Users.Any())
            {
                return;

            }
            ShippingAddress fakeSt = new()
            {
                AddressLine1 = "123 fake st.",
                Name = "Michael Paulson",
                CityTown = "Eugene",
                StateAbr = State.OR,
                PostalCode = "97448",
                Country = "US"
            };
            AppUser michael = new()
            {
                UserName = "michael123",
                Email = "paulsonM@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Michael",
                LName = "Pauslon",
                PhoneNumber = "555-555-5555",
                Address = fakeSt

            };

            AppUser devin = new()
            {
                UserName = "dfreem987",
                Email = "freemand@my.lanecc.edu",
                EmailConfirmed = true,
                FName = "Devin",
                LName = "Freeman",
                Address = new()
                {
                    AddressLine1 = "321 Cedar st.",
                    CityTown = "Junction City",
                    StateAbr = State.OR,
                    PostalCode = "97448",
                    Country = "US",
                    Residential = true,
                    Name = "Devin Freeman"
                },
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
            _ = await userManager!.CreateAsync(devin, "!BassCase987");
            _ = await userManager!.CreateAsync(nehemiah, "@Password123");
            _ = await userManager!.CreateAsync(michael, "@Password123");
            _ = await userManager!.CreateAsync(steven, "@Password123");
            await context.SaveChangesAsync();
        }
    }
}

