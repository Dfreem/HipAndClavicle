
namespace HipAndClavicle.Data
{
    public class SeedShoppingCart
    {
        public static async Task Seed(ApplicationDbContext context)
        {
            if (await context.ShoppingCarts.AnyAsync())
            {
                return;
            }


            var cart = new ShoppingCart { ShoppingCartId = "cart1" };
            var listing1 = await context.Listings.FindAsync(1);
            var listing2 = await context.Listings.FindAsync(2);
            var listing3 = await context.Listings.FindAsync(4);

            if (listing1 != null)
            {
                var cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.ShoppingCartId,
                    ListingId = listing1.ListingId,
                    Listing = listing1,
                    Quantity = 1,
                };
                cart.ShoppingCartItems.Add(cartItem);
            }

            if (listing2 != null)
            {
                var cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.ShoppingCartId,
                    ListingId = listing2.ListingId,
                    Listing = listing2,
                    Quantity = 1,
                };
                cart.ShoppingCartItems.Add(cartItem);
            }

            if (listing3 != null)
            {
                var cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.ShoppingCartId,
                    ListingId = listing3.ListingId,
                    Listing = listing3,
                    Quantity = 1,
                };
                cart.ShoppingCartItems.Add(cartItem);
            }
            await context.ShoppingCarts.AddAsync(cart);
            await context.SaveChangesAsync();

        }
    }
}
