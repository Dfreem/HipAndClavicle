using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HipAndClavicle.Repositories
{
    public static class SeedUsers
    {
        public static async Task Seed(IServiceProvider services)
        {
            const string ANNE_PASS = "@Password123";
            UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
            if (userManager!.Users.Any())
            {
                return;

            }
            ShippingAddress addy1 = new()
            {
                Name = "Anne Smith",
                AddressLine1 = "321 New St",
                CityTown = "Anytown",
                StateAbr = State.OK,
                PostalCode = "12345",
                Country = "USA"
            };
            ShippingAddress fakeSt = new()
            {
                AddressLine1 = "123 fake st.",
                Name = "Michael Paulson",
                CityTown = "Eugene",
                StateAbr = State.OR,
                PostalCode = "97448",
                Country = "US"
            };

            AppUser HCadmin = new()
            {
                UserName = "hcsadmin",
                Email = "hcadmin@hipclavicle.com",
                EmailConfirmed = true,
                FName = "H",
                LName = "Cadmin",
                PhoneNumber = "555-555-5555",
                Address = fakeSt
            };

            AppUser anne = new()
            {
                UserName = "Anne123",
                Email = "anne123@IHopeThisIsntARealEmailDomain.nope",
                EmailConfirmed = true,
                FName = "Anne",
                LName = "Smith",
                Address = addy1,
                PhoneNumber = "1234567890",
            };


            AppUser ann = new()
            {
                UserName = "Ann89",
                Email = "ann89@notarealemail.com",
                EmailConfirmed = true,
                FName = "Ann",
                LName = "Brown",
                Address = addy1,
                PhoneNumber = "9876543210",
            };

            AppUser anneMarie = new()
            {
                UserName = "AnneMarie77",
                Email = "annemarie77@fakemail.net",
                EmailConfirmed = true,
                FName = "Anne-Marie",
                LName = "Jones",
                Address = addy1,
                PhoneNumber = "5555555555",
            };

            AppUser ane = new()
            {
                UserName = "AneDoe",
                Email = "anedoe@somemail.com",
                EmailConfirmed = true,
                FName = "Ane",
                LName = "Doe",
                Address = addy1,
                PhoneNumber = "1111111111",
            };
            AppUser annie = new()
            {
                UserName = "AnnieSmith",
                Email = "anniesmith@anothermail.net",
                EmailConfirmed = true,
                FName = "Annie",
                LName = "Smith",
                Address = addy1,
                PhoneNumber = "2222222222",
            };

            AppUser an = new()
            {
                UserName = "An21",
                Email = "an21@madeupmail.com",
                EmailConfirmed = true,
                FName = "An",
                LName = "Lee",
                Address = addy1,
                PhoneNumber = "3333333333",
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
            AppUser testAdmin123 = new()
            {
                UserName = "TestAdmin123",
                FName = "First",
                LName = "Last",
                Email = "fake@fake.com",
                Address = fakeSt
            };

            _ = await userManager!.CreateAsync(testAdmin123, "@Password123");
            _ = await userManager!.CreateAsync(devin, "!BassCase987");
            _ = await userManager!.CreateAsync(nehemiah, "@Password123");
            _ = await userManager!.CreateAsync(michael, "@Password123");
            _ = await userManager!.CreateAsync(steven, "@Password123");
            _ = await userManager!.CreateAsync(HCadmin, "NiceDragon123!");

            _ = await userManager!.CreateAsync(anne, ANNE_PASS);
            _ = await userManager!.CreateAsync(anneMarie, ANNE_PASS);
            _ = await userManager!.CreateAsync(annie, ANNE_PASS);
            _ = await userManager!.CreateAsync(ane, ANNE_PASS);
            _ = await userManager!.CreateAsync(ann, ANNE_PASS);
            _ = await userManager!.CreateAsync(an, ANNE_PASS);
        }
    }
}

