using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Data
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
            _ = await userManager!.CreateAsync(devin, "!BassCase987");
            _ = await userManager!.CreateAsync(nehemiah, "@Password123");
            _ = await userManager!.CreateAsync(michael, "@Password123");
            _ = await userManager!.CreateAsync(steven, "@Password123");
        }
    }
}

