using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HipAndClavicle.Data
{
    public class SeedShoppingCart
    {
        public static async Task Seed(ApplicationDbContext context, IServiceProvider services)
        {
            if (await context.ShoppingCartItems.AnyAsync())
            {
                return;
            }

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var devin = await userManager.FindByNameAsync("dfreem987");
            var michael = await userManager.FindByNameAsync("michael123");
            var steven = await userManager.FindByNameAsync("steven123");
            var nehemiah = await userManager.FindByNameAsync("nehemiah123");

            ShoppingCart[] carts = {
            new ShoppingCart { CartId = devin.Id, Owner = devin },
            new ShoppingCart { CartId = michael.Id, Owner = michael },
            new ShoppingCart { CartId = steven.Id, Owner = steven },
            new ShoppingCart { CartId = nehemiah.Id, Owner = nehemiah }
            };

            var listing1 = await context.Listings.FirstOrDefaultAsync(p => p.ListingId == 1);
            var listing2 = await context.Listings.FirstOrDefaultAsync(p => p.ListingId == 3);

            for (int i = 0; i < carts.Length; i++)
            {
                var cart = carts[i];

                var shoppingCartItem1 = new ShoppingCartItem
                {
                    ListingItem = listing1,
                    Quantity = 2
                };

                var shoppingCartItem2 = new ShoppingCartItem
                {
                    ListingItem = listing2,
                    Quantity = 1
                };

                cart.ShoppingCartItems = new List<ShoppingCartItem> { shoppingCartItem1, shoppingCartItem2 };
                await context.ShoppingCarts.AddAsync(cart);
            }

            await context.SaveChangesAsync();
        }
    } 
}
